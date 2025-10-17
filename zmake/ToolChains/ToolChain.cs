namespace ZMake.ToolChains;

public sealed class ToolChain
{
    public static Name ZMake = Name.Create(ArtifactName.ZMake, ["toolchain","zmake"]);
    
    public Dictionary<Name, ITool> Tools { get; init; } = [];
    
    
}
