namespace ZMake;

public interface ITarget
{
    IName Name { get; }
    
    IEnumerable<IName> Requirements { get; }

    IEnumerable<ITask> Tasks { get; }
}