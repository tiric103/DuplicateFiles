namespace FileDuplicateLib;

public class Duplicate: IDuplicate
{
    public Duplicate(string filename, long fileSize, string path)
    {
        Filename = filename;
        FileSize = fileSize;
        FilePaths = new List<string>()
        {
            path
        };
    }
    public IEnumerable<string> FilePaths { get; }

    public string Filename { get; }
    
    public long FileSize { get; }
}