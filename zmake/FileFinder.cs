using System.Runtime.InteropServices;

namespace ZMake;

/// <summary>
/// This powerful program finder let you find everything you want.
/// </summary>
public class FileFinder
{
    /// <summary>
    /// Paths that store binary(.exe .lib .a .o) files.
    /// Its item should be absolute path.
    /// </summary>
    public List<string> BinaryPaths { get; init; }
    
    private FileFinder(List<string> binaryPaths)
    {
        BinaryPaths = binaryPaths;
    }

    public static FileFinder CreateEmpty()
    {
        return new FileFinder([]);
    }

    public static FileFinder FromSearchPaths(IEnumerable<string> searchPaths)
    {
        List<string> absPath = [..searchPaths.Select(Path.GetFullPath)];
        return new FileFinder(absPath);
    }

    public static FileFinder FromEnvironment()
    {
        var path = Environment.GetEnvironmentVariable("PATH");
        if (path is null)
        {
            return CreateEmpty();
        }

        var splitter = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';

        return FromSearchPaths(path.Split(splitter).SkipWhile(string.IsNullOrWhiteSpace));
    }

    public IEnumerable<string> SearchProgram(string programName)
    {
        var suffix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : string.Empty;
        
        foreach (var path in BinaryPaths)
        {
            var withOutSuffix = Path.Combine(path, programName);
            if (File.Exists(withOutSuffix))
            {
                yield return withOutSuffix;
            }
            var withSuffix = Path.Combine(path, $"programName{suffix}");
            if (File.Exists(withSuffix))
            {
                yield return withSuffix;
            }
        }
    }
}
