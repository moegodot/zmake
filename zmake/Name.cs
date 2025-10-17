using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace ZMake;

public sealed class Name : IEquatable<Name>
{
    public ArtifactName ArtifactName { get; init; }
    public IReadOnlyList<string> Names { get; init; }

    public static readonly Name ZMake = new Name(ArtifactName.ZMake, ["root"]);
    
    public static readonly Regex NameRegex = new("^[a-z_]+[a-z0-9_]*$");

    private Name(ArtifactName artifactName,string[] names)
    {
        ArtifactName = artifactName;
        Names = names;
    }
    
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Name other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Names.Aggregate(ArtifactName.GetHashCode(), HashCode.Combine);
    }

    public override string ToString()
    {
        return $"{ArtifactName}#{string.Join(':', Names)}";
    }

    public static bool TryCreate(ArtifactName artifactName, IEnumerable<string> names,[NotNullWhen(true)]out Name? result)
    {
        result = null;
        var nameArray = names.ToArray();
        
        foreach (var name in nameArray)
        {
            if (!NameRegex.IsMatch(name))
            {
                return false;
            }
        }

        result = new(artifactName, nameArray);
        return true;
    }

    public static Name Create(ArtifactName artifactName, IEnumerable<string> names)
    {
        if (!TryCreate(artifactName, names, out var result))
        {
            throw new ArgumentException("invalid names format");
        }

        return result;
    }
    
    public static bool TryParse(string str,[NotNullWhen(true)]out Name? result)
    {
        result = null;
        var part = str.Split('#');

        if (part.Length != 2)
        {
            return false;
        }

        if (ArtifactName.TryParse(part[0], out ArtifactName? artifact,out _))
        {
            return TryCreate(artifact, part[1].Split(':'), out result);
        }

        return false;
    }

    public static Name Parse(string str)
    {
        if (!TryParse(str, out var result))
        {
            throw new ArgumentException("invalid name format");
        }

        return result;
    }

    public bool Equals(Name? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ArtifactName.Equals(other.ArtifactName) && Names.SequenceEqual(other.Names);
    }
}