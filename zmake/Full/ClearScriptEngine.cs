using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Serilog;

namespace ZMake.Full;

public class ClearScriptEngine
{
    public ClearScriptEngine(BuildContext context,string targetFile)
    {
        var engine = new V8ScriptEngine($"v8-context-{context.Name}");
        
        engine.AddHostObject("$", HostItemFlags.None);
        
        engine.AddHostObject("$", HostItemFlags.None, engine);
        
        engine.DocumentSettings.AddSystemDocument("","");
        
        engine.DocumentSettings.AddSystemDocument("node:fs",DocumentCategory.Script, "");
        engine.DocumentSettings.AddSystemDocument("node:process",DocumentCategory.Script, "");

        engine.DocumentSettings.Loader = new TypescriptLoader();
        engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
        engine.DocumentSettings.FileNameExtensions += ";ts";
    }
}