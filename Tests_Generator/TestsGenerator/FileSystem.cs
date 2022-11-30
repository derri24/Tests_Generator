namespace Tests_Generator;

public static class FileSystem
{
    public static void CreateFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);
        StreamWriter streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(content);
        streamWriter.Close();
        fileStream.Close();
    }

    public static string ReadFile(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open);
        StreamReader streamReader = new StreamReader(fileStream);
        var content = streamReader.ReadToEnd();
        streamReader.Close();
        fileStream.Close();
        return content;
    }
    public static List<string> GetFilesFromDirectory(string path)
    {
        return Directory.GetFiles(path, "*.cs").ToList();
    }
}