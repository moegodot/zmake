using System.Runtime.InteropServices;

namespace ZMake;

internal static partial class Transformer
{
    [LibraryImport("zmake_transformer", EntryPoint = "transform_typescript",StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint Transform(string ts,string source);
    
    [LibraryImport("zmake_transformer", EntryPoint = "free_transformed_typescript",StringMarshalling = StringMarshalling.Utf8)]
    private static partial void FreeTransformed(nint transformed);

    public static string TransformTypescript(string source, string sourceName)
    {
        string str;
        var result = Transform(source, sourceName);
        try
        {
            str = Marshal.PtrToStringUTF8(result) ?? throw new InvalidDataException("got invalid utf-8 string");
        }
        finally
        {
            FreeTransformed(result);
        }
        return str;
    }
}