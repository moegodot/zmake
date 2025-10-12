using System.Collections.Concurrent;

namespace ZMake;

public sealed class BuildContext
{
    public const string MainContextName = "main";
    
    public required string Name { get; init; }

    public List<Phase> Phases { get; init; }
    
    public ConcurrentDictionary<object, object> Items { get; } = [];

    public event Action<BuildContext,LifecycleEventArgs> LifecycleEvent;

    public BuildContext()
    {
        Phases = Phase.DefaultPhases().ToList();
        Phases.Sort();
    }

    public void Run()
    {
        foreach (var phase in Phases)
        {
            LifecycleEvent(this, new LifecycleEventArgs()
            {
                Phase = phase,
                Sequence = LifecycleSequence.Before
            });
            LifecycleEvent(this, new LifecycleEventArgs()
            {
                Phase = phase,
                Sequence = LifecycleSequence.Current
            });
            LifecycleEvent(this, new LifecycleEventArgs()
            {
                Phase = phase,
                Sequence = LifecycleSequence.After
            });
        }
    }
}
