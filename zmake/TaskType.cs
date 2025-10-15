namespace ZMake;

public enum TaskType
{
    /// <summary>
    /// A task that has IO and cpu operations.
    /// </summary>
    Default = 0,
    /// <summary>
    /// A heavy task like create new compiler processor and compile a file.
    /// This usually was run in a single thread.
    /// </summary>
    Heavy = 1,
    /// <summary>
    /// A task that mainly use IO.
    /// </summary>
    IoBound = 2,
}