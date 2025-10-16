namespace ZMake.Script;

public static class Console
{
    public static void log(params object[] objs)
    {
        info(objs);
    }
    
    public static void info(params object[] objs)
    {
        Serilog.Log.Information(":{Console.Info}",  string.Join(' ',objs.Select(
            s => s.ToString())));
    }
    
    public static void trace(params object[] objs)
    {
        Serilog.Log.Verbose(":{Console.Trace}",  string.Join(' ',objs.Select(
            s => s.ToString())));
    }
    
    public static void warn(params object[] objs)
    {
        Serilog.Log.Warning(":{Console.Warn}",  string.Join(' ',objs.Select(
            s => s.ToString())));
    }
    
    public static void error(params object[] objs)
    {
        Serilog.Log.Error(":{Console.Error}",  string.Join(' ',objs.Select(
            s => s.ToString())));
    }
    
    public static void debug(params object[] objs)
    {
        Serilog.Log.Debug(":{Console.Debug}",  string.Join(' ',objs.Select(
            s => s.ToString())));
    }

    public static void assert(bool condition, params object[] objs)
    {
        if (!condition)
        {
            Serilog.Log.Debug(":Assertion failed:{Console.Debug}",  string.Join(' ',objs.Select(
                s => s.ToString())));
        }
    }
    
    public static void clear()
    {
        return;
    }
}