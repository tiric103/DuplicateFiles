namespace FileDuplicateLib;

public class FileDuplicatesCheck: IFileDuplicatesCheck
{
    public IEnumerable<IDuplicate> Collect_Candidates(string path)
    {
        return Collect_Candidates(path, ComparisonMode.Size_and_name);
    }

    public IEnumerable<IDuplicate> Collect_Candidates(string path, ComparisonMode mode)
    {
        IEqualityComparer<IDuplicate> comparer = new DuplicateEqualityComparer(mode);
        HashSet<IDuplicate> fileHash = new(comparer);
        
        var subDirectories = Directory.GetDirectories(path);
        foreach (var file in Directory.GetFiles(path))
        {
            var fileInfo = new FileInfo(file);
            IDuplicate fileToCheck = new Duplicate(ExtractFileName(file), fileInfo.Length, file);
            if(fileHash.TryGetValue(fileToCheck, out IDuplicate existingFile))
            {
                existingFile.FilePaths.ToList().AddRange(fileToCheck.FilePaths);
            } 
            else fileHash.Add(fileToCheck);
        }

        foreach (var subDir in subDirectories)
        {
            var subResult = Collect_Candidates(subDir, mode);
            foreach (var file in subResult)
            {
                if(fileHash.TryGetValue(file, out IDuplicate existingFile))
                {
                    existingFile.FilePaths.ToList().AddRange(file.FilePaths);
                } 
                else fileHash.Add(file);
            }
        }

        return fileHash;
    }

    public IEnumerable<IDuplicate> Check_Candidates(IEnumerable<IDuplicate> candidates)
    {
        throw new NotImplementedException();
    }

    private string ExtractFileName(string path)
    {
        return path.Split("\\")[^1];
    }
}