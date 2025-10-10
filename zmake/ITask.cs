namespace ZMake;

public interface ITask
{
    ITarget Parent { get; }

    Task Do();
}
