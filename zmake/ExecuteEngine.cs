using Serilog;

namespace ZMake;

public sealed class ExecuteEngine
{
    public int ConcurrentThread { get; init; }
    
    private TaskFactory DefaultScheduler { get; init; }
    
    private TaskFactory IoScheduler { get; init; }
    
    public BuildContext Context { get; init; }

    public ExecuteEngine(BuildContext context)
    {
        Context = context;
        ConcurrentThread = Environment.ProcessorCount;

        if (!ThreadPool.SetMaxThreads(ConcurrentThread,1))
        {
            throw new InvalidOperationException("failed to call ThreadPool.SetMaxThreads");
        }

        DefaultScheduler = new(
            context.AbortToken, 
            TaskCreationOptions.None, 
            TaskContinuationOptions.None,
            TaskScheduler.Default);
        
        IoScheduler = new(
            context.AbortToken, 
            TaskCreationOptions.None, 
            TaskContinuationOptions.None, 
            TaskScheduler.Default);
    }

    private TaskFactory Dispatch(ZTask task)
    {
        if (task.Type == TaskType.IoBound)
        {
            return IoScheduler;
        }

        return DefaultScheduler;
    }

    public Task Execute(ZTask task)
    {
        return Dispatch(task).StartNew(async () =>
        {
            var ret = await task.Run(Context);

            if (ret.Success)
            {
                Log.Verbose("Task {Task} execute {Status}, result: {TaskResult}", 
                    task.ToString(),
                    "succeed",
                    ret);
            }
            else
            {
                Log.Fatal("Task {Task} execute {Status}, result: {TaskResult}",
                    task.ToString(),
                    "failed",
                    ret);
            }
            
            Context.Abort();
        });
    }
}