using System.Reflection;
using Serilog;
using Serilog.Events;

namespace ZMake;

public class Program
{
    private static readonly Version Version = Assembly.GetEntryAssembly()!.GetName().Version
        ?? throw new InvalidProgramException("no version found from entry assembly");

    public static readonly string VersionString = $"{Version.Major}.{Version.Minor}.{Version.Revision}";

    public const string DefaultFile = "makefile.ts";

    private static bool _printBanner = true;

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

    internal static void Help()
    {
        
    }
    
    internal static int Main(string[] args)
    {
        var file = DefaultFile;
        var index = 0;
        var useClearScript = 
#if ZMakeAOT
                false
#else
                true
#endif
            ;
        while (index != args.Length)
        {
            var arg = args[index];
            index++;
            var nextArg = args.Length != index ? args[index] : null;

            if (arg == "--help")
            {
                Help();
                return 0;
            }
            else if (arg == "--makefile")
            {
                file = nextArg ?? throw new ArgumentException($"--makefile require a file like {DefaultFile}");
            }
            else if (arg == "--version")
            {
                Console.WriteLine($"zmake version {VersionString}");
                return 0;
            }
            else if (arg == "--use-clearScript")
            {
#if ZMakeAOT
                throw new InvalidProgramException("can not use clear script in AOT mode");
#endif
                useClearScript = true;
            }
            else if (arg == "--use-jint")
            {
                useClearScript = false;
            }
            else if (arg == "--no-logo")
            {
                _printBanner = false;
            }
            else if (arg == "decompress")
            {
                throw new NotImplementedException();
            }
            else if (arg == "compress")
            {
                throw new NotImplementedException();
            }
            else
            {
                Console.WriteLine($"Unknown argument `{arg}`");
                return 1;
            }
        }

        if (_printBanner)
        {
            Console.WriteLine(Banner);
        }
        
        using var log = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .CreateLogger();
        Log.Logger = log;

        BuildContext context = new()
        {
            Name = BuildContext.MainContextName
        };

        Action<BuildContext> hook;

        #if !ZMakeAOT
        if (useClearScript)
        {
            hook = buildContext =>
            {
                
            };
        }
#else
        if(false){}
#endif
        else
        {
            hook = buildContext =>
            {

            };
        }
        
        context.LifecycleEvent += (context, args) =>
        {
            if (args.Phase != Phase.Initialize)
            {
                hook(context);
            }
        };
        
        try
        {
            context.Start();
            context.Run();
        }
        catch (Exception)
        {
            context.Abort();
            return 1;
        }
        finally
        {
            context.Abort();
        }

        return 0;
    }
}