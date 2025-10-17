using System.Collections.Concurrent;
using System.Drawing;
using Serilog;

namespace ZMake;

public sealed class BuildContext
{
    public const string MainContextName = "main";
    
    public required string Name { get; init; }

    public List<Phase> Phases { get; init; }

    public ConcurrentDictionary<Name, ITarget> Targets { get; init; } = [];
    
    /// <summary>
    /// The root of source code.
    /// </summary>
    public required string BaseDirectory { get; init; }
    
    /// <summary>
    /// The root of the binary.
    /// </summary>
    public required string BuildDirectory { get; init; }
    
    public bool ColoredOutput { get; init; }
    
    public ConcurrentDictionary<object, object> Items { get; } = [];

    public DateTime? StartTime { get; private set; } = null;

    public ExecuteEngine Engine { get; init; }

    public TimeSpan TimeFromStart => 
        !StartTime.HasValue ? throw new InvalidOperationException("the build has not started")
            : DateTime.Now.Subtract(StartTime.Value);

    public event Action<BuildContext, LifecycleEventArgs> LifecycleEvent = (context, args) =>
    {
        Log.Verbose("Lifecycle event at {Context}:{Phase}.{Sequence}",context,args.Phase.Name,args.Sequence.ToString());
    };

    public override string ToString()
    {
        return $"build context `{Name}`";
    }

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
        StartTime = DateTime.Now;
        Log.Verbose(
            "Build context `{Context}` start with arguments: " +
            "BaseDirectory:{BaseDirectory};" +
            "BuildDirectory:{BuildDirectory};" +
            "ColoredOutput:{ColoredOutput};" +
            "StartTime:{StartTime};" +
            "Phases:{Phases};" +
            "Items:{Items};",
            Name,
            BaseDirectory,
            BuildDirectory,
            ColoredOutput,
            StartTime,
            string.Join("->",Phases.Select((p)=>p.Name)),
            string.Join(",",Items.Select(p=>$"{{`{p.Key}`:`{p.Value}`}}")));
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
