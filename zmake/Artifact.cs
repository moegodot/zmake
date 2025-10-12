using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Semver;

namespace ZMake;

/// <summary>
/// Artifact coordinates are most often represented as groupId:artifactId:version.
/// It should be constant.
/// </summary>
public sealed class Artifact : IEquatable<Artifact>
{
    public string GroupId { get; init; }
    public string ArtifactId { get; init; }
    
    /// <summary>
    /// Semver 2.0
    /// </summary>
    public string Version { get; init; }

    internal static readonly Artifact ZMakeArtifact = new Artifact("moe.kawayi.org","zmake",Program.VersionString);

    public static readonly Regex GroupIdRegex = new("^[a-z_]+[a-z0-9_]*(\\.[a-z_]+[a-z0-9_]*)+$");
    public static readonly Regex ArtifactIdRegex = new("^[a-z_]+[a-z0-9_]*$");

    private Artifact(string groupId, string artifactId, string version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(groupId);
        ArgumentException.ThrowIfNullOrWhiteSpace(artifactId);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        GroupId = groupId;
        ArtifactId = artifactId;
        Version = version;
    }

    public static Artifact Parse(string text)
    {
        return !TryParse(text, out var result,out var reason) ? throw new FormatException(reason) : result;
    }

    public static bool TryParse(string text,
        [NotNullWhen(true)]out Artifact? result,
        [NotNullWhen(false)]out string? reason)
    {
        result = null;
        reason = null;
        
        var splitted = text.Split(':');

        if (splitted.Length != 3)
        {
            reason = "can not found two `:` in the string";
            return false;
        }

        return TryCreate(splitted[0], splitted[1],
            splitted[2], out result, out reason);
    }

    public static bool TryCreate(
        string groupId,
        string artifactId,
        string version,
        [NotNullWhen(true)]out Artifact? result,
        [NotNullWhen(false)]out string? reason)
    {
        result = null;
        reason = null;
        
        if (!GroupIdRegex.IsMatch(groupId))
        {
            reason = "invalid groupId format";
            return false;
        }

        if (!ArtifactIdRegex.IsMatch(artifactId))
        {
            reason = "invalid artifactId format";
            return false;
        }

        if (!SemVersion.TryParse(version, out _, maxLength: 8196))
        {
            reason = "invalid version(semver v2.0) format";
            return false;
        }

        result = new Artifact(groupId, artifactId, version);
        return true;
    }

    public static Artifact Create(string groupId,
        string artifactId,
        string version)
    {
        return !TryCreate(groupId,artifactId, version, 
            out var result,out var reason) ? throw new FormatException(reason) : result;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is Artifact other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GroupId, ArtifactId, Version);
    }

    public override string ToString()
    {
        return $"{GroupId}:{ArtifactId}:{Version}";
    }

    public bool Equals(Artifact? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return GroupId == other.GroupId && ArtifactId == other.ArtifactId && Version == other.Version;
    }
}