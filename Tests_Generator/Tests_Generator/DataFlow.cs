using System.Threading.Tasks.Dataflow;

namespace Tests_Generator;

public static class DataFlow
{

    public static async Task Run(int countParallelLoadedFiles,int countParallelProcessedTasks,
        int countParallelWrittenFiles,string directoryPath,string testsDirectoryPath)
    {
        var optionsLoadedFiles = new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = countParallelLoadedFiles};
        var optionsProcessedTasks = new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = countParallelProcessedTasks};
        var optionsWrittenFiles = new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = countParallelWrittenFiles};

        var listFilesBlock = new TransformManyBlock<string, string>(FileSystem.GetFilesFromDirectory);
        var getFilesContentBlock = new TransformBlock<string, string>(FileSystem.ReadFile, optionsLoadedFiles);
        var getGenerateResultsBlock = new TransformBlock<string, Dictionary<string, string>>(TestsGenerator.GetTestDictionary, optionsProcessedTasks);
        var getDictionaryPairsBlock = new TransformManyBlock<Dictionary<string, string>, KeyValuePair<string, string>>
            (dictionary => dictionary.ToList());
        var createTestFileBlockBlock = new ActionBlock<KeyValuePair<string, string>>(
            test => { FileSystem.CreateFile($"{testsDirectoryPath}\\{test.Key}.cs", test.Value); }, optionsWrittenFiles);

        var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
        listFilesBlock.LinkTo(getFilesContentBlock, linkOptions);
        getFilesContentBlock.LinkTo(getGenerateResultsBlock, linkOptions);
        getGenerateResultsBlock.LinkTo(getDictionaryPairsBlock, linkOptions);
        getDictionaryPairsBlock.LinkTo(createTestFileBlockBlock, linkOptions);

        await listFilesBlock.SendAsync(directoryPath);
        listFilesBlock.Complete();
        await createTestFileBlockBlock.Completion;
    }
    
}