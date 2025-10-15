namespace ZMake;

public static class TopologicalSorting
{
    public static ZTask Sort(Dictionary<Name,ITarget> targets,ITarget target)
    {
        Dictionary<Name, bool> status = [];
        return Search(targets, status, target);
    }

    private static ZTask Search(Dictionary<Name, ITarget> targets, Dictionary<Name, bool> status, ITarget target)
    {
        if (status.TryGetValue(target.Name, out var value) && value)
        {
            throw new ArgumentException($"find circular dependency in searching of {target.Name}");
        }
        
        status[target.Name] = true;

        List<ZTask> parent = [];
        
        foreach (var requirementName in target.Requirements)
        {
            if (targets.TryGetValue(requirementName,out var requirement))
            {
                parent.Add(Search(targets, status, requirement));
            }
            else
            {
                throw new InvalidOperationException($"can not find requirement `{requirementName}` of target `{target}` in all targets");
            }
        }
        
        status[target.Name] = false;
        
        return ZTask.CreateWhenAllZTask(target.CreateUnitedTask(), parent);
    }
}