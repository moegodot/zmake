namespace ZMake.ToolChains;

public interface ITool<in T> : ITool
{
    Task<bool> Execute(T argument);
}