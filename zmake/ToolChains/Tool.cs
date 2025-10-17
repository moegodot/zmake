using System.Diagnostics;

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

    protected virtual bool TrySetArguments(ref List<string> arguments)
    {
        return true;
    }

    protected virtual bool TrySetStartInfo(ProcessStartInfo info)
    {
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>null means no exit code check</returns>
    protected virtual int? GetExpectedExitCode()
    {
        return 0;
    }

    public async Task<bool> Execute(IEnumerable<string> arguments)
    {
        var args = arguments.ToList();
        if (!TrySetArguments(ref args))
        {
            return false;
        }

        Process process = new();
        process.StartInfo.ArgumentList.Clear();
        args.ForEach(process.StartInfo.ArgumentList.Add);

        if (!TrySetStartInfo(process.StartInfo))
        {
            return false;
        }

        if (!process.Start())
        {
            return false;
        }

        await process.WaitForExitAsync();

        var expected = GetExpectedExitCode();
        
        return expected == null || process.ExitCode == expected;
    }
}