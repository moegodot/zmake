using System.Collections.Concurrent;
using Serilog;

namespace ZMake;

public sealed class BuildContext
{
    public const string MainContextName = "main";
    
    public required string Name { get; init; }

    public List<Phase> Phases { get; init; }

    public ConcurrentDictionary<Name, ITarget> Targets { get; init; } = [];
    
    public ConcurrentDictionary<object, object> Items { get; } = [];

    public DateTime? StartTime { get; private set; } = null;

    public ExecuteEngine Engine { get; init; }

    public TimeSpan TimeFromStart => 
        !StartTime.HasValue ? throw new InvalidOperationException("the build has not started")
            : DateTime.Now.Subtract(StartTime.Value);

    public event Action<BuildContext, LifecycleEventArgs> LifecycleEvent = (context, args) => { };

    private readonly CancellationTokenSource _abortToken = new();

    public CancellationToken AbortToken => _abortToken.Token;

    public BuildContext()
    {
        Phases = Phase.DefaultPhases().ToList();
        Phases.Sort();
        Engine = new(this);
    }

    public void Start()
    {
        if (StartTime.HasValue)
        {
            throw new InvalidOperationException("the build has started");
        }
        Log.Information("Building context {Context} started",Name);
        StartTime = DateTime.Now;
    }

    public void Abort()
    {
        if (!_abortToken.IsCancellationRequested)
        {
            _abortToken.Cancel();
        }
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
