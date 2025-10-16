namespace ZMake;

public interface IResolver
{
    
    string? BaseDirectory { get; }

    void Resolve(string file);
}