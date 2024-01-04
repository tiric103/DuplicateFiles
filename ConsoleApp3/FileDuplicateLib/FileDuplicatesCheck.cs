using System.Security.Cryptography;

namespace FileDuplicateLib;

public class FileDuplicatesCheck: IFileDuplicatesCheck
{
    public IEnumerable<IDuplicate> Collect_Candidates(string path)
    {
        return Collect_Candidates(path, ComparisonMode.Size_and_name);
    }

    public IEnumerable<IDuplicate> Collect_Candidates(string path, ComparisonMode mode)
    {
        var fileHash = CheckDirectory(path, mode);

        //filter out all IDuplicates that have only one path, as they can not be duplicates
        foreach (var duplicate in fileHash.Where(duplicate => duplicate.FilePaths.Count() < 2))
        {
            fileHash.Remove(duplicate);
        }
        
        return fileHash;
    }

    private HashSet<IDuplicate> CheckDirectory(string path, ComparisonMode mode)
    {
        //set up
        IEqualityComparer<IDuplicate> comparer = new DuplicateEqualityComparer(mode);
        HashSet<IDuplicate> fileHash = new(comparer);

        //iterate all files in this directory
        foreach (var file in Directory.GetFiles(path))
        {
            var fileInfo = new FileInfo(file);
            IDuplicate fileToCheck = new Duplicate(Path.GetFileName(file), fileInfo.Length, file);
            if (fileHash.TryGetValue(fileToCheck, out IDuplicate existingFile))
            {
                existingFile.FilePaths.ToList().AddRange(fileToCheck.FilePaths);
            }
            else fileHash.Add(fileToCheck);
        }

        //apply Collect_Candidates to all directories of the current one
        foreach (var subDir in Directory.GetDirectories(path))
        {
            var subResult = CheckDirectory(subDir, mode);
            foreach (var file in subResult)
            {
                if (fileHash.TryGetValue(file, out IDuplicate existingFile))
                {
                    var tmpList = existingFile.FilePaths.ToList();
                    tmpList.AddRange(file.FilePaths);
                    existingFile.FilePaths = tmpList;
                }
                else fileHash.Add(file);
            }
        }

        return fileHash;
    }

    public IEnumerable<IDuplicate> Check_Candidates(IEnumerable<IDuplicate> candidates)
    {
        List<IDuplicate> result = new();
        foreach (var candidate in candidates)
        {
            List<string> md5Hashes = candidate.FilePaths.Select(filePath => GetMD5Hash(filePath)).ToList();
            List<string> tempPath = candidate.FilePaths.ToList();

            while (md5Hashes.Count > 0)
            {
                var hashToCheck = md5Hashes[0];
                md5Hashes.RemoveAt(0);
                IDuplicate duplicate = new Duplicate(candidate.Filename, candidate.FileSize, tempPath.ElementAt(0));
                tempPath.RemoveAt(0);
                int index = 0;
                while (index != -1)
                {
                    index = md5Hashes.FindIndex(x => x.Equals(hashToCheck));
                    if (index != -1)
                    {
                        duplicate.FilePaths = duplicate.FilePaths.Append(tempPath.ElementAt(index));
                        md5Hashes.RemoveAt(index);
                        tempPath.RemoveAt(index);
                    }
                }
                result.Add(duplicate);
            }
        }
        
        //filter out all IDuplicates that have only one path, as they can not be duplicates
        result = result.Where(x => x.FilePaths.Count() > 1).ToList();

        return result;
    }

    private static string GetMD5Hash(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}