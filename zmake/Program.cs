using System.Reflection;

namespace ZMake;

public class Program
{
    public static readonly Version Version = Assembly.GetEntryAssembly()!.GetName().Version
        ?? throw new InvalidProgramException("no version found from entry assembly");

    public static readonly string VersionString = $"{Version.Major}.{Version.Minor}.{Version.Revision}";
    
    public static void Main(string[] args)
    {
        
    }
}