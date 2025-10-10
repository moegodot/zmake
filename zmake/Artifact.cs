namespace ZMake;

public sealed class Artifact : IArtifact, IEquatable<IArtifact>
{
    public string GroupId { get; init; }
    public string ArtifactId { get; init; }
    public string Version { get; init; }

    public Artifact(string groupId, string artifactId, string version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(groupId);
        ArgumentException.ThrowIfNullOrWhiteSpace(artifactId);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        GroupId = groupId;
        ArtifactId = artifactId;
        Version = version;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is IArtifact other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GroupId, ArtifactId, Version);
    }

    public override string ToString()
    {
        return $"{GroupId}:{ArtifactId}:{Version}";
    }

    public bool Equals(IArtifact? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return GroupId == other.GroupId && ArtifactId == other.ArtifactId && Version == other.Version;
    }
}