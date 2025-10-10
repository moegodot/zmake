using System.Collections.Concurrent;

namespace ZMake;

public sealed class BuildContext
{
    public const string MainContextName = "main";
    
    public required string Name { get; init; }
    
    public ConcurrentDictionary<object, object> Items { get; } = [];
}