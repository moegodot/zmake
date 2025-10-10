namespace ZMake;

/// <summary>
/// Stands for a globally unique name.
/// It should be constant.
/// </summary>
public interface IName : IEquatable<IName>
{
    IArtifact Artifact { get; }
    IEnumerable<string> Names { get; }
}