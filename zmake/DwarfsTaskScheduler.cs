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

    public DwarfsTaskScheduler(CancellationToken token,int threadCount,ThreadPriority priority)
    {
        MaximumConcurrencyLevel = threadCount;
        while (threadCount != 0)
        {
            var t = new Thread(WorkerLoop)
            {
                Name = $"[{nameof(DwarfsTaskScheduler)}(IO task scheduler)]-{threadCount}",
                Priority = priority,
            };
            t.Start();
            threadCount--;
        }

        _token = CancellationTokenSource
            .CreateLinkedTokenSource(token,
            _source.Token).Token;
    }

    public override int MaximumConcurrencyLevel { get; }

    private void WorkerLoop()
    {
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
            catch (Exception)
            {
                // TODO:LOG
                // TODO:REPORT
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
        _source.Dispose();
    }
}