namespace ZMake;

public sealed class TaskResult
{
    public required ZTask Source { get; init; }
    
    public required TimeSpan Elapsed { get; init; }
    
    public required TimeSpan StartTime { get; init; }
    
    public required Exception? Exception { get; init; }
    
    public required bool Canceled { get; init; }

    public bool Failed => Exception != null;

    public bool Success => (!Canceled) && (Exception == null);
}
