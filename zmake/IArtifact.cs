namespace ZMake;

/// <summary>
/// Artifact coordinates are most often represented as groupId:artifactId:version.
/// It should be constant.
/// </summary>
public interface IArtifact : IEquatable<IArtifact>
{
    string GroupId { get; }
    string ArtifactId { get; }
    string Version { get; }
    
}