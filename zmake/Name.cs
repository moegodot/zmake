namespace ZMake;

public class Name : IName
{
    public required IArtifact Artifact { get; init; }
    public required IEnumerable<string> Names { get; init; }
    
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is IName other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Artifact, Names.GetHashCode());
    }

    public override string ToString()
    {
        return $"{Artifact}:{string.Join(':', Names)}";
    }

    public bool Equals(IName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Artifact.Equals(other.Artifact) && Names.SequenceEqual(other.Names);
    }
}