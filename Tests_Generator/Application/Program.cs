using System.Threading.Tasks.Dataflow;
using Tests_Generator;

string GetFilesDirectoryPath()
{
    Console.WriteLine("Enter files directory path: ");
    return Console.ReadLine();
}

List<string> GetFilesFromDirectory(string path)
{
    return Directory.GetFiles(path, "*.cs").ToList();
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

int GetCountParallelLoadedFiles()
{
    Console.WriteLine("Enter count parallel loaded files: ");
    return Convert.ToInt32(Console.ReadLine());
}

int GetCountParallelProcessedTasks()
{
    Console.WriteLine("Enter count parallel processed tasks: ");
    return Convert.ToInt32(Console.ReadLine());
}

int GetCountParallelWrittenFiles()
{
    Console.WriteLine("Enter count parallel written files: ");
    return Convert.ToInt32(Console.ReadLine());
}

async Task Main()
{
    var countParallelLoadedFiles = GetCountParallelLoadedFiles();
    var countParallelProcessedTasks = GetCountParallelProcessedTasks();
    var countParallelWrittenFiles = GetCountParallelWrittenFiles();

    var directoryPath = GetFilesDirectoryPath();
    var testsDirectoryPath = GetTestsDirectoryPath();
    
    var optionsLoadedFiles = new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = countParallelLoadedFiles};
    var optionsProcessedTasks = new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = countParallelProcessedTasks};
    var optionsWrittenFiles = new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = countParallelWrittenFiles};

    var listFilesBlock = new TransformManyBlock<string, string>(GetFilesFromDirectory);
    var getFilesContentBlock = new TransformBlock<string, string>(ReadFileContent, optionsLoadedFiles);
    var getGenerateResultsBlock = new TransformBlock<string, Dictionary<string, string>>(TestsGenerator.GetTestDictionary, optionsProcessedTasks);
    var getDictionaryPairsBlock = new TransformManyBlock<Dictionary<string, string>, KeyValuePair<string, string>>
        (dictionary => dictionary.ToList());
    var createTestFileBlockBlock = new ActionBlock<KeyValuePair<string, string>>(
        test => { CreateTestFile($"{testsDirectoryPath}\\{test.Key}.cs", test.Value); }, optionsWrittenFiles);

    var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
    listFilesBlock.LinkTo(getFilesContentBlock, linkOptions);
    getFilesContentBlock.LinkTo(getGenerateResultsBlock, linkOptions);
    getGenerateResultsBlock.LinkTo(getDictionaryPairsBlock, linkOptions);
    getDictionaryPairsBlock.LinkTo(createTestFileBlockBlock, linkOptions);

    await listFilesBlock.SendAsync(directoryPath);
    listFilesBlock.Complete();
    await createTestFileBlockBlock.Completion;
    Console.WriteLine("Test generation is successful!!!");
}

await Main();