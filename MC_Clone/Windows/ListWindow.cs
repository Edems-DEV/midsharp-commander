using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class ListWindow : Window
{
    private FilePanel pane1; //useless?
    private FilePanel pane2;
    readonly List<FilePanel> _panels = new List<FilePanel>();
    private int _activePanelIndex;
    public delegate void OnKey(ConsoleKeyInfo key);
    public event OnKey KeyPress;
    int maxNameLength = 20;

    public ListWindow()
    {
        FilesService service = new FilesService(Config.FILE);
        string[] staticHeader = new string[] { "Name", "Size", "Date" }; //chage me

        this.pane1 = new FilePanel(staticHeader, 0, 4); //service.Headers()
        this.pane2 = new FilePanel(staticHeader, 55, 4); //table.LineLength() + 2 //55
        this._panels.Add(pane1);
        this._panels.Add(pane2);

        //foreach (string[] item in service.Data())
        //{
        //    table.Add(item);
        //}

        foreach (var pane in _panels)
        {
            ImportRows(pane, Config.FOLDER);
        }
        _activePanelIndex = 0;
        this._panels[this._activePanelIndex].Active = true;
        KeyPress += this._panels[this._activePanelIndex].HandleKey;

        //this.pane1.RowSelected += Table_RowSelected;
    }

    public void ChangeActivePanel()
    {
        this._panels[this._activePanelIndex].Active = false;
        KeyPress -= this._panels[this._activePanelIndex].HandleKey;
        //this._panels[this._activePanelIndex].UpdateContent(false);

        this._activePanelIndex++;
        if (this._activePanelIndex >= this._panels.Count)
        {
            this._activePanelIndex = 0;
        }

        this._panels[this._activePanelIndex].Active = true;
        KeyPress += this._panels[this._activePanelIndex].HandleKey;
        //this._panels[this._activePanelIndex].UpdateContent(false);
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
        this.pane1.Draw();
        this.pane2.Draw();
        string[] labels = { "Help", "Menu", "View", "Edit", "Copy", "RenMov", "Mkdir", "Delete", "PullDn", "Quit" };
        var footer = new Footer(labels);
        footer.Draw();
    }

    public override void HandleKey(ConsoleKeyInfo info)
    {
        if (info.Key == ConsoleKey.Tab)
        {
            ChangeActivePanel();
        }
        switch (info.Key)
        {
            case ConsoleKey.DownArrow:
                goto case ConsoleKey.PageUp;
            case ConsoleKey.UpArrow:
                goto case ConsoleKey.PageUp;
            case ConsoleKey.End:
                goto case ConsoleKey.PageUp;
            case ConsoleKey.Home:
                goto case ConsoleKey.PageUp;
            case ConsoleKey.PageDown:
                goto case ConsoleKey.PageUp;
            case ConsoleKey.PageUp:
                this.KeyPress(info);
                break;
        }
        //this.pane1.HandleKey(info);
        
    }
}
