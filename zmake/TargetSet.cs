using System.Collections;
using Serilog;

namespace ZMake;

public sealed class TargetSet : IEnumerable<ITarget>
{
    public List<ITarget> All { get; set; } = [];

    public List<ITarget> Build { get; set; } = [];

    public List<ITarget> Test { get; set; } = [];

    public List<ITarget> Install { get; set; } = [];

    public void Add(ITarget target,string category)
    {
        All.Add(target);
        switch (category)
        {
            case "build":
                Build.Add(target);
                break;
            case "test":
                Test.Add(target);
                break;
            case "install":
                Install.Add(target);
                break;
            default:
                Log.Warning("Unknown target name for target:{Target}",target);
                break;
        }
    }

    public IEnumerator<ITarget> GetEnumerator()
    {
        return All.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
