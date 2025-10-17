using System.Text;
using System.Text.Json.Nodes;
using Acornima.Ast;
using Jint;
using Jint.Native;
using Jint.Native.Promise;
using Jint.Runtime;
using Jint.Runtime.Modules;
using Serilog;
using ZMake.Script;
using Console = ZMake.Script.Console;

namespace ZMake;

public class JintScriptEngine : IResolver
{
    
    public Engine Engine { get; init; }

    public string? BaseDirectory { get; init; }

    public JintScriptEngine(string name,string baseDir)
    {
        Options options = new();
        options.Strict = true;
        BaseDirectory = Path.GetFullPath(baseDir);
        options.Modules.ModuleLoader = new ZMakeLoader(BaseDirectory);
         var engine = InitiateEngine(new Engine(options));
         Engine = engine;
         Log.Verbose(
             "Create Jint engine `{Name}` with base directory `{BaseDirectory}`",
             name,
             BaseDirectory);
    }
    
    public static Engine InitiateEngine(Engine engine)
    {
        engine.Modules.Add("node:fs", (builder) =>
        {
            builder.ExportFunction(nameof(Fs.readFileSync), (args) =>
            {
                ZMakeScriptException.ThrowIfArgumentNotMatch(
                    nameof(Fs.readFileSync),
                    args,
                    Types.String,
                    Types.String);
                return Fs.readFileSync(args[0].AsString(), args[1].AsString());
            });
            builder.ExportFunction(nameof(Fs.writeFileSync), (args) =>
            {
                ZMakeScriptException.ThrowIfArgumentNotMatch(
                    nameof(Fs.writeFileSync),
                    args,
                    Types.String,
                    Types.String);
                Fs.writeFileSync(args[0].AsString(), args[1].AsString());
            });
            builder.ExportFunction(nameof(Fs.existsSync), (args) =>
            {
                ZMakeScriptException.ThrowIfArgumentNotMatch(
                    nameof(Fs.existsSync),
                    args,
                    Types.String);
                Fs.existsSync(args[0].AsString());
            });
            builder.ExportFunction(nameof(Fs.mkdirSync), (args) =>
            {
                ZMakeScriptException.ThrowIfArgumentNotMatch(
                    nameof(Fs.mkdirSync),
                    args,
                    Types.String);
                Fs.mkdirSync(args[0].AsString());
            });
            builder.ExportFunction(nameof(Fs.readdirSync), (args) =>
            {
                ZMakeScriptException.ThrowIfArgumentNotMatch(
                    nameof(Fs.readdirSync),
                    args,
                    Types.String);
                var items = Fs.readdirSync(args[0].AsString())
                    .Select((s) => (JsValue)new JsString(s)).ToArray();
                var array = new JsArray(engine, items);
                return array;
            });
        });

        engine.Modules.Add("node:console", (builder) =>
        {
            builder.ExportFunction(nameof(Console.trace), (values) =>
            {
                Console.trace(values.Select((value)=>(object)value.ToString()).ToArray());
            });
            builder.ExportFunction(nameof(Console.debug), (values) =>
            {
                Console.debug(values.Select((value)=>(object)value.ToString()).ToArray());
            });
            builder.ExportFunction(nameof(Console.info), (values) =>
            {
                Console.info(values.Select((value)=>(object)value.ToString()).ToArray());
            });
            builder.ExportFunction(nameof(Console.log), (values) =>
            {
                Console.log(values.Select((value)=>(object)value.ToString()).ToArray());
            });
            builder.ExportFunction(nameof(Console.warn), (values) =>
            {
                Console.warn(values.Select((value)=>(object)value.ToString()).ToArray());
            });
            builder.ExportFunction(nameof(Console.error), (values) =>
            {
                Console.error(values.Select((value)=>(object)value.ToString()).ToArray());
            });
            builder.ExportFunction(nameof(Console.assert), (values) =>
            {
                Console.assert(values[0].AsBoolean(),
                    values[1..].Select((value)=>(object)value.ToString()).ToArray());
            });
            builder.ExportFunction(nameof(Console.clear), (values) =>
            {
                Console.clear();
            });
        });

        var console = engine.Modules.Import("node:console");
        engine.Global.Set("console", console);
        
        return engine;
    }

    public void Resolve(string path)
    {
        path = Path.GetFullPath(path);
        Engine.Modules.Import(path);
    }
}
