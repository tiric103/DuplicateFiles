using FileDuplicateLib;

var fileChecker = new FileDuplicatesCheck();

var result = fileChecker.Collect_Candidates("C:\\Users\\n655061\\Downloads\\Test");

var newResult = fileChecker.Check_Candidates(result);