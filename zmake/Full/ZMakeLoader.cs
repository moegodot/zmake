using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Serilog;

namespace ZMake.Full;

public class ZMakeLoader : DefaultDocumentLoader
{

    public Dictionary<string, string> SpecialModules { get; init; } = [];
    
    public override async Task<Document> LoadDocumentAsync(
        DocumentSettings settings,
        DocumentInfo? sourceInfo,
        string specifier,
        DocumentCategory category,
        DocumentContextCallback contextCallback)
    {
        if (SpecialModules.TryGetValue(specifier, out var doc))
        {
            return new StringDocument(new DocumentInfo()
            {
                Category = ModuleCategory.Standard
            }, doc);
        }

        Log.Verbose("Try load document {Specifier}",specifier);
        
        if (!Path.HasExtension(specifier))
        {
            throw new NotSupportedException("Unsupported Directory Import");
        }
        
        var document = await base.LoadDocumentAsync(settings, sourceInfo, specifier, category, contextCallback);

        if (document.Info.Category == DocumentCategory.Json || document is not StringDocument javascript)
        {
            Log.Verbose("Not transform {Document}", document.Info.Uri);
            return document;
        }
        var typescript = Transformer.TransformTypescript(javascript.StringContents, javascript.Info.Uri.ToString());
        
        Log.Verbose("Transform typescript {Document}", document.Info.Uri);
        
        // replace original javascript cache
        return CacheDocument(new StringDocument(javascript.Info, typescript), true);
    }
}