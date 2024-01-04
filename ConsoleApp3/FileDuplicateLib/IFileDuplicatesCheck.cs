namespace FileDuplicateLib;

public interface IFileDuplicatesCheck
{
    IEnumerable<IDuplicate> Collect_Candidates(string path);
    IEnumerable<IDuplicate> Collect_Candidates(string path, ComparisonMode mode);
	
    IEnumerable<IDuplicate> Check_Candidates(IEnumerable<IDuplicate> candidates);
}