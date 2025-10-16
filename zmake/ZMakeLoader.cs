using Jint;
using Jint.Runtime.Modules;

namespace ZMake;

public sealed class ZMakeLoader(string basePath) 
    : DefaultModuleLoader(Path.GetFullPath(basePath), false)
{
    protected override string LoadModuleContents(Engine engine, ResolvedSpecifier resolved)
    {
        var content = base.LoadModuleContents(engine, resolved);
        
        var fileName = Uri.UnescapeDataString(resolved.Uri!.AbsolutePath);
        if (fileName.EndsWith(".ts") || (!resolved.ModuleRequest.IsJsonModule()))
        {
            return Transformer.TransformTypescript(content, fileName);
        }

        return content;
    }
}