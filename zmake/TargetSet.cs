using System.Collections;
using Serilog;

namespace ZMake;

public sealed class TargetSet : IEnumerable<ITarget>
{
    public IReadOnlyList<ITarget> All { get; }

    public IReadOnlyList<ITarget> Build { get; }

    public IReadOnlyList<ITarget> Test { get; }

    public IReadOnlyList<ITarget> Install { get; }

    public TargetSet(List<ITarget> all, List<ITarget> build, List<ITarget> test, List<ITarget> install)
    {
        All = all.ToArray();
        Build = build.ToArray();
        Test = test.ToArray();
        Install = install.ToArray();
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
