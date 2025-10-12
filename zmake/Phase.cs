namespace ZMake;

public record Phase(int Priority) : IComparable<Phase>
{
    public string Name { get; init; }
    
    public static readonly Phase Validate = new Phase(nameof(Validate), 100);
    public static readonly Phase Initialize = new Phase(nameof(Initialize), 200);
    public static readonly Phase GenerateSources = new Phase(nameof(GenerateSources), 300);
    public static readonly Phase ProcessSources = new Phase(nameof(ProcessSources), 400);
    public static readonly Phase GenerateResources = new Phase(nameof(GenerateResources), 500);
    public static readonly Phase ProcessResources = new Phase(nameof(ProcessResources), 600);
    public static readonly Phase Compile = new Phase(nameof(Compile), 700);
    public static readonly Phase ProcessCompiled = new Phase(nameof(ProcessCompiled), 800);
    public static readonly Phase GenerateTestSources = new Phase(nameof(GenerateTestSources), 900);
    public static readonly Phase ProcessTestSources = new Phase(nameof(ProcessTestSources), 1000);
    public static readonly Phase CompileTest = new Phase(nameof(CompileTest), 1100);
    public static readonly Phase ProcessCompiledTest = new Phase(nameof(ProcessCompiledTest), 1200);
    public static readonly Phase Test = new Phase(nameof(Test), 1300);
    public static readonly Phase Package = new Phase(nameof(Package), 1400);
    public static readonly Phase IntegrationTest = new Phase(nameof(IntegrationTest), 1500);
    public static readonly Phase Verify = new Phase(nameof(Verify), 1600);
    public static readonly Phase Install = new Phase(nameof(Install), 1700);
    public static readonly Phase Deploy = new Phase(nameof(Deploy), 1800);

    public static IEnumerable<Phase> DefaultPhases()
    {
        return
        [
            Validate, Initialize, GenerateSources,
            ProcessSources, GenerateResources, ProcessResources, Compile,
            ProcessCompiled, GenerateTestSources, ProcessTestSources, CompileTest,
            ProcessCompiledTest,
            Test,
            Package,
            IntegrationTest,Verify,Install,Deploy
        ];
    }

    public Phase(string name, int priority) : this(priority)
    {
        Name = name;
    }

    public int CompareTo(Phase? other)
    {
        return Priority.CompareTo(other?.Priority);
    }
}
