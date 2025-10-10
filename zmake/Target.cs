namespace ZMake;

public sealed class Target : ITarget
{
    public IEnumerable<IName> Name { get; init; } = [];
    public IEnumerable<IName> Requirements { get; init; } = [];
    public IEnumerable<ITask> Tasks { get; init; } = [];

    public Target(IEnumerable<IName> name,
        IEnumerable<IName> requirements, 
        IEnumerable<ITask> tasks)
    {
        Name = name;
        Requirements = requirements;
        Tasks = tasks;
    }
}