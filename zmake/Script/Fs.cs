using System.Text;

namespace ZMake.Script;

public static class Fs
{
    public static string readFileSync(string path,string encoding)
    {
        return File.ReadAllText(path, Encoding.GetEncoding(encoding));
    }
    public static void writeFileSync(string path, string content)
    {
        File.WriteAllText(path, content, Encoding.UTF8);
    }
    public static bool existsSync(string path)
    {
        return Path.Exists(path);
    }
    public static void mkdirSync(string dir)
    {
        Directory.CreateDirectory(dir);
    }
    public static string[] readdirSync(string dir)
    {
        return Directory.EnumerateFileSystemEntries(dir).ToArray();
    }
}