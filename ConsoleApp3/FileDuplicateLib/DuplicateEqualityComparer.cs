namespace FileDuplicateLib;

public class DuplicateEqualityComparer: IEqualityComparer<IDuplicate>
{
    private readonly ComparisonMode _mode;
    public DuplicateEqualityComparer(ComparisonMode mode)
    {
        _mode = mode;
    }
    public bool Equals(IDuplicate? x, IDuplicate? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        if (_mode == ComparisonMode.Size) return x.FileSize.Equals(y.FileSize);
        return x.Filename == y.Filename && x.FileSize.Equals(y.FileSize);
    }

    public int GetHashCode(IDuplicate obj)
    {
        return _mode switch
        {
            ComparisonMode.Size => HashCode.Combine(obj.FileSize),
            _ => HashCode.Combine(obj.Filename.ToLowerInvariant(), obj.FileSize)
        };
    }
}