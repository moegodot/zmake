namespace ZMake.ToolChains;

public static class Cpp
{
    public static Name Compiler =  Name.Create(ArtifactName.ZMake, ["toolchain","cxx","compiler"]);
}