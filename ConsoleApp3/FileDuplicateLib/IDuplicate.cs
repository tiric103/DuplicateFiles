namespace FileDuplicateLib;

public interface IDuplicate
{
    IEnumerable<string> FilePaths { get; set; }
    string Filename { get; }
    long FileSize { get; }
}