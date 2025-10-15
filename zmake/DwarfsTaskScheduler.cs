using Serilog;

namespace ZMake;

/// <summary>
/// This simple task scheduler suits for small tasks like IO tasks.
/// It uses its own thread pool instead of <see cref="System.Threading.ThreadPool"/>.
/// </summary>
public sealed class DwarfsTaskScheduler : TaskScheduler,IDisposable
{
    private readonly Lock _lock = new();
    
    private readonly LinkedList<Task> _tasks = [];

    private readonly CancellationTokenSource _source = new();

    private readonly CancellationToken _token;

    private readonly Thread[] _workers;

    private const string ThreadNamePrefix = nameof(DwarfsTaskScheduler);

    public DwarfsTaskScheduler(int threadCount,ThreadPriority priority,CancellationToken token)
    {
        MaximumConcurrencyLevel = threadCount;
        _workers = new Thread[threadCount];
        while (threadCount != 0)
        {
            var t = new Thread(WorkerLoop)
            {
                Name = $"{ThreadNamePrefix}-unknown",
                Priority = priority,
                IsBackground = true,
            };
            t.Start();
            threadCount--;
            _workers[threadCount] = t;
        }

        _token = CancellationTokenSource
            .CreateLinkedTokenSource(token,
            _source.Token).Token;
    }

    public override int MaximumConcurrencyLevel { get; }

    private void WorkerLoop()
    {
        Thread.CurrentThread.Name = $"{ThreadNamePrefix}-{Environment.CurrentManagedThreadId}";
        
        while (!_token.IsCancellationRequested)
        {
            try
            {
                Task? task = null;

                lock (_lock)
                {
                    if (_tasks.Count != 0)
                    {
                        task = _tasks.First!.Value;
                        _tasks.RemoveFirst();
                    }
                }

                if (task is null)
                {
                    Thread.Yield();
                }
                else
                {
                    TryExecuteTask(task);
                }
            }
            catch (OperationCanceledException) when (_token.IsCancellationRequested)
            {
                // do nothing
            }
            catch (Exception exception)
            {
                Log.Error(exception, "get an error when execute a task");
            }
        }
    }
    
    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler.getscheduledtasks?view=net-9.0
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    protected override IEnumerable<Task>? GetScheduledTasks()
    {
        var lockTaken = false;
        try
        {
            lockTaken = _lock.TryEnter();
            return lockTaken ? _tasks.ToArray() : throw new NotSupportedException();
        }
        finally
        {
            if (lockTaken)
                _lock.Exit();
        }
    }

    protected override void QueueTask(Task task)
    {
        lock (_lock)
        {
            _tasks.AddLast(task);
        }
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        if (!(Thread.CurrentThread.Name ?? string.Empty).StartsWith(ThreadNamePrefix))
            return false;
        if (!taskWasPreviouslyQueued)
            return TryExecuteTask(task);
        if (TryDequeue(task))
            return TryExecuteTask(task);

        return false;
    }

    protected override bool TryDequeue(Task task)
    {
        lock (_lock)
        {
            return _tasks.Remove(task);
        }
    }

    public void Dispose()
    {
        _source.Cancel();
        
        try
        {
            _ = _workers.Select(thread => thread.Join(5000)).ToArray();
        }
        catch (AggregateException)
        {
            // 忽略取消异常
        }
        
        _source.Dispose();
    }
}