using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class ListWindow : Window
{
    private FilePanel table;
    private FilePanel table2;
    int maxNameLength = 20;
    string path_config = @"C:\Users\root\Desktop\x"; //Config.FILE;

    public ListWindow()
    {
        FilesService service = new FilesService(Config.FILE);
        string[] staticHeader = new string[] { "Name", "Size", "Date" }; //chage me

        this.table = new FilePanel(staticHeader, 0, 4); //service.Headers()
        this.table2 = new FilePanel(staticHeader, 55, 4); //table.LineLength() + 2 //55

        //foreach (string[] item in service.Data())
        //{
        //    table.Add(item);
        //}

        ImportRows(table, path_config); //my
        ImportRows(table2, @"C:\Users\root\Desktop"); //my

        this.table.RowSelected += Table_RowSelected;
    }
    
    void ImportRows(FilePanel pane, string path)
    {
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
            pane.Add(new string[] { @"\" + Truncated(di.Name, maxNameLength - 1), size.ToString(), di.LastWriteTime.ToString("MMM dd HH:mm") });
        }
        //Files
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            FileInfo fi = new FileInfo(file);
            pane.Add(new string[] { Truncated(fi.Name, maxNameLength), fi.Length.ToString(), fi.LastWriteTime.ToString("MMM dd HH:mm") });
        }
    }
    string Truncated(string ts, int maxLength, string trun = "~")
    {
        if (ts.Length < maxLength)
            return ts;

        maxLength = maxLength - trun.Length;
        int a = maxLength / 2 + maxLength % 2;
        int b = maxLength / 2;
        var truncated = ts.Substring(0, a) + trun + ts.Substring(ts.Length - b, b);

        return truncated;
    }

    private void Table_RowSelected(int index)
    {
        this.Application.SwitchWindow(new EditWindow(index));
    }

    public override void Draw()
    {
        this.table.Draw();
        this.table2.Draw();
        string[] labels = { "Help", "Menu", "View", "Edit", "Copy", "RenMov", "Mkdir", "Delete", "PullDn", "Quit" };
        var footer = new Footer(labels);
        footer.Draw();
    }

    public override void HandleKey(ConsoleKeyInfo info)
    {
        this.table.HandleKey(info);
    }
}
