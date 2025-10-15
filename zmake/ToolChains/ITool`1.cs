namespace ZMake.ToolChains;

public interface ITool<in T> : ITool
{
    bool Execute(T argument);
}