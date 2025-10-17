namespace ZMake.ToolChains;

public interface ITool
{
    string Program { get; }
    
    string? Version { get; }

    Task<bool> Execute(IEnumerable<string> arguments);
}