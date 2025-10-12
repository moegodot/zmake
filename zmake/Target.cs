namespace ZMake;

public sealed class Target : ITarget
{
    public Name Name { get; init; }
    public IEnumerable<Name> Requirements { get; init; }
    public IEnumerable<ITask> Tasks { get; init; }

    public Target(Name name,
        IEnumerable<Name> requirements, 
        IEnumerable<ITask> tasks)
    {
        Name = name;
        Requirements = requirements;
        Tasks = tasks;
    }
}