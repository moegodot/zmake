namespace ZMake;

public sealed class Target : ITarget
{
    public Name Name { get; init; }
    public IEnumerable<Name> Requirements { get; init; }
    public IEnumerable<ZTask> Tasks { get; init; }
    
    public Target(Name name,
        IEnumerable<Name> requirements, 
        IEnumerable<ZTask> tasks)
    {
        Name = name;
        Requirements = requirements.ToArray();
        Tasks = tasks.ToArray();
    }

    public override string ToString()
    {
        return $"(Target {Name})";
    }
}