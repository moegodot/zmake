namespace ZMake.ToolChains;

public interface ITool
{
    string Program { get; }
    
    string? Version { get; }

    bool Execute(IEnumerable<string> arguments);
}