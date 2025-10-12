namespace ZMake;

public sealed class Name : IEquatable<Name>
{
    public required Artifact Artifact { get; init; }
    public required IEnumerable<string> Names { get; init; }
    
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Name other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Artifact, Names.GetHashCode());
    }

    public override string ToString()
    {
        return $"{Artifact}:{string.Join(':', Names)}";
    }

    public bool Equals(Name? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Artifact.Equals(other.Artifact) && Names.SequenceEqual(other.Names);
    }
}