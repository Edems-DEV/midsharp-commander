namespace MC_Clone;
internal class MyFileService
{
    public string Path { get; set; }
    public FileInfo File { get; set; }
    

    public MyFileService(string path)
    {
        Path = path;
    }
    public MyFileService(FileInfo file)
    {
        File = file;
        Path = file.FullName;
    }

    private List<char[]> Read()
    {
        List<char[]> result = new List<char[]>();

        using (StreamReader reader = new StreamReader(Path))
        {
            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine().ToArray());
            }
        }

        return result;
    }
}
