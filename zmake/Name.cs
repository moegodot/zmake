using System.Text.RegularExpressions;

namespace ZMake;

public sealed class Name : IEquatable<Name>
{
    public Artifact Artifact { get; init; }
    public IEnumerable<string> Names { get; init; }

    public static readonly Name ZMake = new Name(Artifact.ZMake, ["root"]);
    
    public static readonly Regex NameRegex = new("^[a-z_]+[a-z0-9_]*$");

    public Name(Artifact artifact,IEnumerable<string> names)
    {
        Names = names.ToArray();
        Artifact = artifact;

        foreach (var name in Names)
        {
            if (!NameRegex.IsMatch(name))
            {
                throw new ArgumentException($"the name `{name}` do not match NameRegex",nameof(names));
            }   
        }
    }
    
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
        return $"{Artifact}::{string.Join(':', Names)}";
    }

    public bool Equals(Name? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Artifact.Equals(other.Artifact) && Names.SequenceEqual(other.Names);
    }
}