using System.Diagnostics;

namespace ZMake;

public sealed class ZTask
{
    public ITarget Target { get; init; }
    public string DisplayTaskName { get; init; }
    public Func<BuildContext,Task> Task{ get; init; }
    public TaskType Type { get; init; }

    public override string ToString()
    {
        return $"({Target} Task {DisplayTaskName})";
    }

    private static async Task<TaskResult> WrappedTask(Func<BuildContext,Task> action,BuildContext context,ZTask zTask)
    {
        var canceled = false;
        Exception? error = null;
        Stopwatch stopwatch = new();
        stopwatch.Start();
        var startTime = context.TimeFromStart;
        try
        {
            await action(context);
        }
        catch (TaskCanceledException)
        {
            canceled = true;
        }
        catch (Exception exception)
        {
            error = exception;
        }
        finally
        {
            stopwatch.Stop();
        }
            
        return new TaskResult()
        {
            Source = zTask,
            StartTime = startTime,
            Elapsed = stopwatch.Elapsed,
            Exception = error,
            Canceled = canceled
        };
    }

    public async Task<TaskResult> Run(BuildContext context)
    {
        var t = WrappedTask(Task, context, this);
        await t.ConfigureAwait(false);
        return await t;
    }
    
    public ZTask(ITarget from,TaskType type,string displayName,Func<BuildContext,Task> task)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(from);
        ArgumentException.ThrowIfNullOrEmpty(displayName);
        DisplayTaskName = displayName;
        Target = from;
        Task = task;
        Type = type;
    }
}