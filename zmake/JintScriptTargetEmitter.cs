using System.Text;
using System.Text.Json.Nodes;
using Acornima.Ast;
using Jint;
using Jint.Native;
using Jint.Runtime;

namespace ZMake;

public class JintScriptTargetEmitter : ITargetEmitter
{
    public string Specific { get; init; }
    
    public string? Script { get; init; }

    private record JintScript(string ScriptPath)
    {
        
    }

    public JintScriptTargetEmitter(string scriptPath)
    {
        Specific = Path.GetFullPath(scriptPath);
        Script = null;
    }

    public JintScriptTargetEmitter(string script, string specific)
    {
        Specific = specific;
        Script = script;
    }

    public static Engine InitiateEngine(Engine engine,List<ITask> results)
    {
        engine.Modules.Add("zmake:internal", builder =>
        {
            builder.ExportValue("version", Program.VersionString);
        });
        
        return engine;
    }

    private string GetScriptContent()
    {
        if (Script == null)
        {
            return File.ReadAllText(Specific!, Encoding.UTF8);
        }

        return Script;
    }

    private bool IsInMemory()
    {
        return Script == null;
    }

    private string GetScriptName()
    {
        return Specific;
    }

    private Options GetOptions()
    {
        Options opt = new();

        if (File.Exists(Specific))
        {
            opt.EnableModules(Path.GetDirectoryName(Specific)!);
        }

        return opt;
    }

    private async Task<IEnumerable<ITask>> Evaluate(
        BuildContext context,
        CancellationToken cancellationToken)
    {
        List<ITask> tasks = [];
        
        var engine = InitiateEngine(new Engine(GetOptions()), tasks);
        
        engine.Modules.Add(GetScriptName(), GetScriptContent());
        var results = engine.Modules.Import(GetScriptName());

        List<Artifact> artifacts = [];

        if (results.IsArray())
        {
            foreach (var artifact in results.AsArray())
            {
                var obj = artifact.AsObject();
                
                artifacts.Add(Artifact.Create(
                    obj[nameof(Artifact.GroupId)].AsString(),
                    obj[nameof(Artifact.ArtifactId)].AsString(),
                    obj[nameof(Artifact.Version)].AsString()));
            }
        }
        else
        {
            var obj = results.AsObject();
            artifacts.Add(Artifact.Create(
                obj[nameof(Artifact.GroupId)].AsString(),
                obj[nameof(Artifact.ArtifactId)].AsString(),
                obj[nameof(Artifact.Version)].AsString()));
        }

        return tasks;
    }
    
    public Task<IEnumerable<ITask>> Emit(
        BuildContext context,
        CancellationToken cancellationToken)
    {
        return (Task<IEnumerable<ITask>>)
            context.Items.GetOrAdd(new JintScript(Specific), 
            () => Evaluate(context, cancellationToken));
    }
}
