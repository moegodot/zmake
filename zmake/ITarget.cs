namespace ZMake;

public interface ITarget
{
    IEnumerable<IName> Name { get; }
    
    IEnumerable<IName> Requirements { get; }

    IEnumerable<ITask> Tasks { get; }
}
