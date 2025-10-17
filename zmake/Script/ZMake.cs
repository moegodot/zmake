namespace ZMake.Script;

public class ZMake(BuildContext context)
{
    public BuildContext Context { get; init; } = context;

    public string getProperty(string name)
    {
        return name switch
        {
            "base directory" => Context.BaseDirectory,
            "build directory" => Context.BuildDirectory,
            "context name" => Context.Name,
            "start time" => Context.StartTime.ToString()
            ?? throw new InvalidOperationException("build context not started"),
            _ => throw new ArgumentException("unknown property name")
        };
    }

    public bool getOptions(string name)
    {
        return name switch
        {
            "colored output" => context.ColoredOutput,
            _ => throw new ArgumentException("unknown property name")
        };
    }

    public void abortBuilding(string reason)
    {
        Context.Abort();
        throw new InvalidOperationException($"abort building, reason:{reason}");
    }
}