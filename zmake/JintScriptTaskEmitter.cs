using System.Text;
using Jint;
using Jint.Native;
using Jint.Runtime;

namespace ZMake;

public class JintScriptTaskEmitter : ITaskEmitter
{
    public string Name { get; init; }
    
    public string? Script { get; init; }

    private record JintScript(string ScriptPath)
    {
    }

    public JintScriptTaskEmitter(string scriptPath)
    {
        Name = Path.GetFullPath(scriptPath);
        Script = null;
    }

    public JintScriptTaskEmitter(string script, string name)
    {
        Name = name;
        Script = script;
    }

    public static Engine InitiateEngine(Engine engine,List<ITask> results)
    {
        engine.Modules.Add("zmake", builder =>
        {
            builder.ExportValue("version", new JsString());
            
            var funcName = "requireVersion";
            builder.ExportFunction(funcName, (JsValue[] args) =>
            {
                ZMakeScriptException.ThrowIfArgumentsCountWrong(funcName,args,1);
                ZMakeScriptException.ThrowIfArgumentsTypeWrong(funcName,args,1, Types.String);

                var requireVersion = Version.Parse(args[0].AsString());
                
                if (Program.Version < requireVersion)
                {
                    throw new ZMakeScriptException(
                        $"require zmake version {requireVersion} but run at version {Program.Version}");
                }
            });
            
            builder.ExportType<Artifact>();
        });
        
        return engine;
    }

    private string GetScriptContent()
    {
        if (Script == null)
        {
            return File.ReadAllText(Name!, Encoding.UTF8);
        }

        return Script;
    }

    private string GetScriptName()
    {
        return Name;
    }

    private Options GetOptions()
    {
        Options opt = new();

        opt.EnableModules(Path.GetDirectoryName(Name)!);

        return opt;
    }

    private async Task<IEnumerable<ITask>> Evaluate(
        BuildContext context,
        CancellationToken cancellationToken)
    {
        List<ITask> tasks = [];
        
        var engine = InitiateEngine(new Engine(GetOptions()), tasks);

        engine.Execute(GetScriptContent(), GetScriptName());

        return tasks;
    }
    
    public Task<IEnumerable<ITask>> Emit(
        BuildContext context,
        CancellationToken cancellationToken)
    {
        return (Task<IEnumerable<ITask>>)
            context.Items.GetOrAdd(new JintScript(Name), 
            () => Evaluate(context, cancellationToken));
    }
}
