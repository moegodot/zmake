using System.Collections.Frozen;

namespace ZMake.Script;

public static class Process
{
    public static Dictionary<string,string> 
        GetEnvironmentVariables()
    {
        Dictionary<string, string> dict = [];
        var env = Environment
            .GetEnvironmentVariables();
        
        foreach (var key in env.Keys)
        {
            // TODO:PROCESS THIS
            dict[key.ToString()!] = env[key]!.ToString()
                ?? throw new ApplicationException("get null environment");
        }

        return dict;
    }
    
    public static void
        SetEnvironmentVariable(string key,string value)
    {
        Environment.SetEnvironmentVariable(key,value);
    }
    
}