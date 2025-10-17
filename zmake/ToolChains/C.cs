namespace ZMake.ToolChains;

public static class C
{
    public static Name Compiler = Name.Create(ArtifactName.ZMake, ["toolchain","c","compiler"]);
    
    public static Name Preprocessor =  Name.Create(ArtifactName.ZMake, ["toolchain","c","preprocessor"]);
}