namespace ZMake;

public class LifecycleEventArgs : EventArgs
{
    public required Phase Phase { get; init; }

    public required LifecycleSequence Sequence { get; init; }
}
