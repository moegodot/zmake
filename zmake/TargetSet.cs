namespace ZMake;

public sealed class TargetSet
{
    public List<ITarget> All { get; set; }
    
    public List<ITarget>? Build { get; set; }
    
    public List<ITarget>? Test { get; set; }
}
