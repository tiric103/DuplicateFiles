using FileDuplicateLib;

var fileChecker = new FileDuplicatesCheck();

var result = fileChecker.Collect_Candidates("C:\\Users\\n655061\\Downloads\\Test");

foreach (var item in result)
{
    Console.WriteLine(item.Filename);
}
