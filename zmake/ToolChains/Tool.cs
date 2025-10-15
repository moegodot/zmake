namespace ZMake.ToolChains;

public class Tool(string program) : ITool
{
    public string Program { get; } = program;

    private string? _version = null;
    private bool _gotVersion = false;

    public string? Version
    {
        get
        {
            if (!_gotVersion)
            {
                _gotVersion = true;
                _version = Helper.TryGetVersionOfProgram(Program);
            }

            return _version;
        }
    }

    public bool Execute(IEnumerable<string> arguments)
    {
        
        
        throw new NotImplementedException();
    }
}