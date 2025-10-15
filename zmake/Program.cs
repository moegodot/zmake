using System.Reflection;
using Serilog;
using Serilog.Events;

namespace ZMake;

public class Program
{
    private static readonly Version Version = Assembly.GetEntryAssembly()!.GetName().Version
        ?? throw new InvalidProgramException("no version found from entry assembly");

    public static readonly string VersionString = $"{Version.Major}.{Version.Minor}.{Version.Revision}";

    private const string Banner =
"""
$$$$$$$$\    $$\      $$\     $$$$$$\     $$\   $$\    $$$$$$$$\ 
\____$$  |   $$$\    $$$ |   $$  __$$\    $$ | $$  |   $$  _____|
    $$  /    $$$$\  $$$$ |   $$ /  $$ |   $$ |$$  /    $$ |      
   $$  /     $$\$$\$$ $$ |   $$$$$$$$ |   $$$$$  /     $$$$$\    
  $$  /      $$ \$$$  $$ |   $$  __$$ |   $$  $$<      $$  __|   
 $$  /       $$ |\$  /$$ |   $$ |  $$ |   $$ |\$$\     $$ |      
$$$$$$$$\    $$ | \_/ $$ |   $$ |  $$ |   $$ | \$$\    $$$$$$$$\ 
\________|   \__|     \__|   \__|  \__|   \__|  \__|   \________|
""";

    internal static void Main(string[] args)
    {
        using var log = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .CreateLogger();
        Log.Logger = log;

        Console.WriteLine(Banner);

        BuildContext context = new()
        {
            Name = BuildContext.MainContextName
        };

        try
        {
            context.Start();
            context.Run();
        }
        catch (Exception)
        {
            context.Abort();
        }
        finally
        {
            context.Abort();
        }
    }
}