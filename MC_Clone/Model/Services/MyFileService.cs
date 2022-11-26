namespace MC_Clone;
public class MyFileService
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

    public List<string> Read()
    {
        List<string> result = new List<string>();

        using (StreamReader reader = new StreamReader(Path))
        {
            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine());
            }
        }

        return result;
    }
    public void OverWrite() { }

    #region Stats calc
    public void CountWords() { }
    public void CountLines() { }


    #endregion

}
