using System.Text;
using Jint;

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
            builder.ExportFunction("", (string version) =>
            {
                if (Program.Version < Version.Parse(version))
                {
                    
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
