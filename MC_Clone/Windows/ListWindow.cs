using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class ListWindow : Window
{
    private Table table;

    public ListWindow()
    {
        FilesService service = new FilesService(Config.FILE);
        string[] staticHeader = new string[] { "Name", "Size", "Date" }; //chage me

        this.table = new Table(staticHeader); //service.Headers()

        //foreach (string[] item in service.Data())
        //{
        //    table.Add(item);
        //}

        ImportRows();

        this.table.RowSelected += Table_RowSelected;
    }

    void ImportRows()
    {
        string path = @"C:\Users\root\Desktop"; //@"C:\Users\root\Desktop\Example Folder"
        //Directories
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            long size = 0;
            foreach (var item in di.GetFiles())
            {
                size += item.Length;
            }
            table.Add(new string[] { @"\" + di.Name, size.ToString(), di.LastWriteTime.ToString("MMM dd HH:mm") });
        }
        //Files
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            FileInfo fi = new FileInfo(file);
            table.Add(new string[] { fi.Name, fi.Length.ToString(), fi.LastWriteTime.ToString("MMM dd HH:mm") });
        }
    }

    private void Table_RowSelected(int index)
    {
        this.Application.SwitchWindow(new EditWindow(index));
    }

    public override void Draw()
    {
        this.table.Draw();
    }

    public override void HandleKey(ConsoleKeyInfo info)
    {
        this.table.HandleKey(info);
    }
}
