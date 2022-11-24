using Tests_Generator;


string GetFilesDirectoryPath()
{
    Console.WriteLine("Enter files directory path: ");
    return Console.ReadLine();
}

List<FileInfo> GetFilesFromDirectory(string path)
{
    DirectoryInfo directoryInfo = new DirectoryInfo(path);
    return directoryInfo.GetFiles().ToList();
}

string ReadFileContent(string path)
{
    FileStream fileStream = new FileStream(path, FileMode.Open);
    StreamReader streamReader = new StreamReader(fileStream);
    var content = streamReader.ReadToEnd();
    streamReader.Close();
    fileStream.Close();
    return content;
}

string GetTestsDirectoryPath()
{
    Console.WriteLine("Enter tests directory path: ");
    return Console.ReadLine();
}

void CreateTestFile(string path, string content)
{
    FileStream fileStream = new FileStream(path, FileMode.Create);
    StreamWriter streamWriter = new StreamWriter(fileStream);
    streamWriter.Write(content);
    streamWriter.Close();
    fileStream.Close();
}

void Main()
{
    var directoryPath = GetFilesDirectoryPath();
    Dictionary<string, string> tests = new Dictionary<string, string>();

    var filePaths = GetFilesFromDirectory(directoryPath);
    foreach (var filePath in filePaths)
    {
        var content = ReadFileContent($"{directoryPath}\\{filePath.Name}");
        var resultTests = TestsGenerator.GetTestDictionary(content);
        foreach (var resultTest in resultTests)
            tests.Add(resultTest.Key, resultTest.Value);
    }
    var testsDirectoryPath = GetTestsDirectoryPath();
    foreach (var test in tests)
        CreateTestFile($"{testsDirectoryPath}\\{test.Key}.txt", test.Value);
    Console.WriteLine("Test generation is successful!!!");
}

Main();