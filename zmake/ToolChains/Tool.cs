namespace ZMake.ToolChains;

public abstract class Tool
{
    public string Program { get; init; }

    public bool Execute();
}