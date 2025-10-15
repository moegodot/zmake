namespace ZMake.ToolChains;

public static class C
{
    public static Name Compiler = Name.Create(Artifact.ZMake, ["toolchain","c","compiler"]);
    
    public static Name Preprocessor =  Name.Create(Artifact.ZMake, ["toolchain","c","preprocessor"]);
}