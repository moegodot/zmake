namespace ZMake;

public interface ITargetEmitter
{
    Task<IEnumerable<ZTask>> Emit(
        BuildContext context,
        CancellationToken cancellationToken);
}
