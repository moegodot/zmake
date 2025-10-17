using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using Serilog;
using ZMake.Script;
using Console = System.Console;

namespace ZMake.Full;

public class ClearScriptEngine : IResolver
{
    
    public V8ScriptEngine Engine { get; init; }

    public string? BaseDirectory { get; init; }

    public ClearScriptEngine(string name,string baseDir)
    {
        var engine = new V8ScriptEngine($"v8-context-{name}");
        engine.AllowReflection = true;

        BaseDirectory = Path.GetFullPath(baseDir);
        engine.DocumentSettings.SearchPath = BaseDirectory;

        var loader = new ZMakeLoader();
        engine.DocumentSettings.Loader = loader;

        engine.AddHostType("console",typeof(Script.Console));
        engine.AddHostType("__Z_MAKE_NODE_FS", typeof(Fs));
        loader.SpecialModules["node:fs"] = 
            """
            export const readFileSync = __Z_MAKE_NODE_FS.readFileSync;
            export const writeFileSync = __Z_MAKE_NODE_FS.writeFileSync;
            export const existsSync = __Z_MAKE_NODE_FS.existsSync;
            export const mkdirSync = __Z_MAKE_NODE_FS.mkdirSync;
            export const readdirSync = __Z_MAKE_NODE_FS.readdirSync;
            """;
        engine.AddHostType("__Z_MAKE_NODE_PROCESS",typeof(Process));
        loader.SpecialModules["node:process"] =
            """
            let envList = __Z_MAKE_NODE_PROCESS.GetEnvironmentVariables();
            let handler = {
              set(target, property, value) {
                __Z_MAKE_NODE_PROCESS.SetEnvironmentVariable(property,value);
                target[property] = value;
              },
              get(target, prop){
                return target[prop];
              }
            };
            export let env = new Proxy(envList, handler);
            """;
        
        engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;
        engine.DocumentSettings.FileNameExtensions += ";ts";

        Engine = engine;
        Log.Verbose(
            "Create ClearScript engine `{Name}` with base directory `{BaseDirectory}`",
            name,
            BaseDirectory);
    }

    public void Resolve(string filePath)
    {
        filePath = Path.GetFullPath(filePath);
        Engine.ExecuteDocument(filePath, ModuleCategory.Standard);
    }
}