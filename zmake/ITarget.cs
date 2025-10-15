namespace ZMake;

public interface ITarget
{
    Name Name { get; }
    
    IEnumerable<Name> Requirements { get; }

    IEnumerable<ZTask> Tasks { get; }

    /// <summary>
    /// A simple way to execute all tasks in the <see cref="Tasks"/>
    /// </summary>
    /// <returns></returns>
    ZTask CreateUnitedTask()
    {
        return ZTask.CreateWhenAllZTask(
            new ZTask(this, TaskType.Default, "Tasks of a target", Task.FromResult),
            Tasks);
    }
}
