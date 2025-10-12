namespace ZMake;

public interface ITask
{
    ITarget Parent { get; }
    
    TaskType Type { get; }

    Task Do();
}
