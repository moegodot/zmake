using System.Reflection;

namespace ZMake;

public class Program
{
    public static readonly Version Version = Assembly.GetEntryAssembly()!.GetName().Version
        ?? throw new InvalidProgramException("no version found from entry assembly");

    public static readonly string VersionString = $"{Version.Major}.{Version.Minor}.{Version.Revision}";

    public const string Banner =
"""
$$$$$$$$\ $$\      $$\  $$$$$$\  $$\   $$\ $$$$$$$$\ 
\____$$  |$$$\    $$$ |$$  __$$\ $$ | $$  |$$  _____|
    $$  / $$$$\  $$$$ |$$ /  $$ |$$ |$$  / $$ |      
   $$  /  $$\$$\$$ $$ |$$$$$$$$ |$$$$$  /  $$$$$\    
  $$  /   $$ \$$$  $$ |$$  __$$ |$$  $$<   $$  __|   
 $$  /    $$ |\$  /$$ |$$ |  $$ |$$ |\$$\  $$ |      
$$$$$$$$\ $$ | \_/ $$ |$$ |  $$ |$$ | \$$\ $$$$$$$$\ 
\________|\__|     \__|\__|  \__|\__|  \__|\________|
""";
    
    public static void Main(string[] args)
    {
        Console.Write(Banner);

        BuildContext context = new()
        {
            Name = BuildContext.MainContextName
        };
        
        context.Run();
        
        TaskFactory taskFactory = new(TaskCreationOptions.None, TaskContinuationOptions.None);

        //taskFactory.StartNew();
    }
}