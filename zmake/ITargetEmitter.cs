namespace ZMake;

public interface ITargetEmitter
{
    Task<IEnumerable<ITask>> Emit(
        BuildContext context,
        CancellationToken cancellationToken);
}
