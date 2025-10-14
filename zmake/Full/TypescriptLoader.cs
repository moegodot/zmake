using System.Collections.Concurrent;
using Microsoft.ClearScript;
using Serilog;

namespace ZMake.Full;

public class TypescriptLoader : DefaultDocumentLoader
{
    public override async Task<Document> LoadDocumentAsync(
        DocumentSettings settings,
        DocumentInfo? sourceInfo,
        string specifier,
        DocumentCategory category,
        DocumentContextCallback contextCallback)
    {
        Log.Verbose("Try load document {Specifier}",specifier);
        var document = await base.LoadDocumentAsync(settings, sourceInfo, specifier, category, contextCallback);

        if (document.Info.Category != DocumentCategory.Script || document is not StringDocument javascript)
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