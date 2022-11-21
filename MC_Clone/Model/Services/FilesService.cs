using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class FilesService
{
    public string File { get; set; }

    public FilesService(string file)
    {
        this.File = file;
    }

    public string[] Headers()
    {
        return this.Read()[0];
    }

    public List<string[]> Data()
    {
        List<string[]> result = this.Read();

        result.RemoveAt(0);

        return result;
    }

    public void Update(string[] row, int index)
    {
        List<string[]> data = this.Read();

        data[index + 1] = row;

        using (StreamWriter writer = new StreamWriter(this.File))
        {
            foreach (string[] item in data)
            {
                writer.WriteLine(string.Join(';', item));
            }
        }
    }

    private List<string[]> Read()
    {
        List<string[]> result = new List<string[]>();

        using (StreamReader reader = new StreamReader(this.File))
        {
            while (!reader.EndOfStream)
            {
                result.Add(reader.ReadLine()!.Split(';'));
            }
        }

        return result;
    }
}
