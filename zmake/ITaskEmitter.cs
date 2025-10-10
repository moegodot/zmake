namespace ZMake;

public interface ITaskEmitter
{
    Task<IEnumerable<ITask>> Emit(
        BuildContext context,
        CancellationToken cancellationToken);
}
