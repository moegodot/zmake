using System.Diagnostics;
using Serilog;

namespace ZMake;

public sealed class ZTask
{
    public ITarget Target { get; init; }
    public string DisplayTaskName { get; init; }
    public Func<BuildContext,Task> Func{ get; init; }
    public TaskType Type { get; init; }

    public override string ToString()
    {
        return $"({Target} Task:{DisplayTaskName})";
    }

    private class ZTaskContext(
        Func<BuildContext,Task> action,
        BuildContext context,
        ZTask zTask)
    {
        public Func<BuildContext, Task> Action { get; init; } = action;
        public BuildContext Context { get; init; } = context;
        public ZTask ZTask { get; init; } = zTask;
    }

    private static async Task<TaskResult> WrappedTask(object? rawArgument)
    {
        var argument = (ZTaskContext)rawArgument!;
        var action = argument.Action;
        var context = argument.Context;
        var zTask = argument.ZTask;
        var canceled = false;
        Exception? error = null;
        Stopwatch stopwatch = new();

        if (context.AbortToken.IsCancellationRequested)
        {
            return new TaskResult()
            {
                Source = zTask,
                StartTime = context.TimeFromStart,
                Elapsed = new TimeSpan(0),
                Exception = error,
                Canceled = true
            };
        }
        
        stopwatch.Start();
        var startTime = context.TimeFromStart;
        try
        {
            await action(context);
        }
        catch (OperationCanceledException)
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

        var fmtArgs = new object?[]{zTask,startTime , stopwatch.Elapsed, error ,canceled };
        
        if (error != null)
        {
            Log.Error(Logging.TaskExecuted,fmtArgs);
        }
        else
        {
            Log.Verbose(Logging.TaskExecuted,fmtArgs);
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

    public Task<Task<TaskResult>> Execute(
        BuildContext context,
        TaskFactory factory)
    {
        return factory.StartNew(
            WrappedTask,
            new ZTaskContext(Func, context, this));
    }
    
    public ZTask(ITarget from,TaskType type,string displayName,Func<BuildContext,Task> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        ArgumentNullException.ThrowIfNull(from);
        ArgumentException.ThrowIfNullOrEmpty(displayName);
        DisplayTaskName = displayName;
        Target = from;
        Func = func;
        Type = type;
    }

    public static ZTask CreateWhenAllZTask(ZTask parent, params IEnumerable<ZTask> children)
    {
        return new ZTask(parent.Target, parent.Type, parent.DisplayTaskName, async (context) =>
        {
            var tasks = await Task.WhenAll(children.Select(task => context.Engine.Execute(task)).ToArray());
            var results = await Task.WhenAll(tasks);

            var failure = results.FirstOrDefault((t) => !t.Success);
            
            if(failure != null)
            {
                // do not execute
                Log.Verbose("Not execute task {Task} for the failure of dependent task {Dependency}",
                    parent,failure);
                throw new OperationCanceledException();
            }
            
            await parent.Func(context);
        });
    }
}