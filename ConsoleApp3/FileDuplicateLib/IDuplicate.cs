namespace FileDuplicateLib;

public interface IDuplicate
{
    IEnumerable<string> FilePaths { get; }
    string Filename { get; }
    long FileSize { get; }
}