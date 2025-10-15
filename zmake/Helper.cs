using System.Diagnostics;

namespace ZMake;

public static class Helper
{
    /// <summary>
    /// Extract version from the output of program.
    /// </summary>
    public static string? TryExtractVersion(string output)
    {
        output = output.ToLowerInvariant();
        var outputs = output.Trim().Split();

        foreach (var item in outputs)
        {
            var trimmed = item.TrimStart('v');

            if (trimmed.Length > 2 && char.IsAsciiDigit(trimmed[0]) && trimmed.Contains('.'))
            {
                return trimmed;
            }
        }

        return null;
    }
    
    public static string? TryGetVersionOfProgram(string program)
    {
        var proc = new Process();
        proc.StartInfo.FileName = program;
        proc.StartInfo.Arguments = "--version";
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.RedirectStandardError = true;
        proc.StartInfo.RedirectStandardInput = true;

        if (!proc.Start())
        {
            throw new InvalidOperationException($"failed to start program `{program}`");
        }
        
        proc.StandardInput.Close();

        proc.WaitForExit();

        return TryExtractVersion(proc.StandardOutput.ReadToEnd())
               ?? TryExtractVersion(proc.StandardError.ReadToEnd());
    }
}