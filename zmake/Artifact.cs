namespace ZMake;

public sealed class Artifact
{
    public ArtifactName Name { get; }
    public TargetSet Set { get; }

    public Artifact(ArtifactName name,TargetSet set)
    {
        Name = name;
        Set = set;

        foreach (var target in set)
        {
            if (target.Name.ArtifactName != name)
            {
                throw new ArgumentException(
                    $"Find target `{target}` with Art");
            }
        }
    }
}