namespace ZMake.ToolChains;

public static class Cpp
{
    public static Name Compiler =  Name.Create(Artifact.ZMake, ["toolchain","cxx","compiler"]);
}