using System.Diagnostics;
using Serilog;

namespace ZMake;

public sealed class ExecuteEngine
{
    public int ConcurrentThread { get; init; }
    
    private TaskFactory DefaultFactory { get; init; }
    
    private TaskFactory HeavyFactory { get; init; }
    
    private TaskFactory IoFactory { get; init; }
    
    public BuildContext Context { get; init; }

    public ExecuteEngine(BuildContext context)
    {
        Context = context;
        ConcurrentThread = Environment.ProcessorCount;

        DefaultFactory = new(
            context.AbortToken,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);
        
        IoFactory = new(
            context.AbortToken,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            new DwarfsTaskScheduler(1, ThreadPriority.BelowNormal, context.AbortToken));
        
        HeavyFactory = new(
            context.AbortToken,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            new TargetJobScheduler(ConcurrentThread, ThreadPriority.AboveNormal,IoFactory ,context.AbortToken));
    }

    private TaskFactory Dispatch(TaskType type)
    {
        if (type == TaskType.IoBound)
        {
            return IoFactory;
        }

        if (type == TaskType.Heavy)
        {
            return HeavyFactory;
        }

        return DefaultFactory;
    }

    public Task Execute(Action task, TaskType? type = null)
    {
        type ??= TaskType.Default;
        return Dispatch(type.Value).StartNew(task);
    }

    public Task<Task<TaskResult>> Execute(ZTask task)
    {
        return task.Execute(Context, Dispatch(task.Type));
    }

    public TaskResult ExecuteAndBlockingWait(ZTask task)
    {
        var wrapped = Execute(task);
        wrapped.ConfigureAwait(false);
        wrapped.Wait();
        var result = wrapped.Result;
        result.ConfigureAwait(false);
        result.Wait();
        return result.Result;
    }
}