using Tests_Generator;

string GetFilesDirectoryPath()
{
    Console.WriteLine("Enter files directory path: ");
    return Console.ReadLine();
}

string GetTestsDirectoryPath()
{
    Console.WriteLine("Enter tests directory path: ");
    return Console.ReadLine();
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
    await DataFlow.Run(
        GetCountParallelLoadedFiles(),
        GetCountParallelProcessedTasks(),
        GetCountParallelWrittenFiles(),
        GetFilesDirectoryPath(),
        GetTestsDirectoryPath());

    Console.WriteLine("Test generation is successful!!!");
}

await Main();