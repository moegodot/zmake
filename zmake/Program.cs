using System.Reflection;

namespace ZMake;

public class Program
{
    public static Version Version = Assembly.GetEntryAssembly()!.GetName().Version
        ?? throw new InvalidProgramException("no version found from entry assembly");
    
    public static void Main(string[] args)
    {
        
    }
}