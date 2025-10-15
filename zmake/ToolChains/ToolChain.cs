namespace ZMake.ToolChains;

public sealed class ToolChain
{
    public static Name ZMake = Name.Create(Artifact.ZMake, ["toolchain","zmake"]);
    
    public Dictionary<Name, ITool> Tools { get; init; } = [];
    
    
}
