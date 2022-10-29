using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class ListWindow : Window
{
    readonly List<FilePanel> _panels = new List<FilePanel>();
    private int _activePanelIndex;
    public delegate void OnKey(ConsoleKeyInfo key);
    public event OnKey KeyPress;
    public int winWidth = 0;
    FilePanel ActivePanel()
    {
        return _panels[_activePanelIndex];
    }

    public ListWindow()
    {
        winWidth = Console.BufferWidth / 2;
        FilesService service = new FilesService(Config.FILE);
        string[] staticHeader = new string[] { "Name", "Size", "Date" }; //chage me //{ "Name", "Size", "Date" }

        
        FilePanel pane1 = new FilePanel(Config.FILE,staticHeader, 0, 4); //service.Headers()
        FilePanel pane2 = new FilePanel(Config.FILE,staticHeader, winWidth, 4); //table.LineLength() + 2 //55
        this._panels.Add(pane1);
        this._panels.Add(pane2);

        foreach (var pane in _panels)
        {
            pane.LineLength();
            pane.ImportRows(Config.FOLDER);
            pane.RowSelected += Table_RowSelected;

        }
        _activePanelIndex = 0;
        ActivePanel().Active = true;
        KeyPress += ActivePanel().HandleKey;
    }

    public void ChangeActivePanel()
    {
        ActivePanel().Active = false;
        KeyPress -= ActivePanel().HandleKey;
        //ActivePanel().UpdateContent(false);

        this._activePanelIndex++;
        if (this._activePanelIndex >= this._panels.Count)
        {
            this._activePanelIndex = 0;
        }

        ActivePanel().Active = true;
        KeyPress += ActivePanel().HandleKey;
        //ActivePanel().UpdateContent(false);
    }

    private void Table_RowSelected(int index)
    {
        this.Application.SwitchWindow(new EditWindow(index));
    }

    public override void Draw()
    {
        winWidth = Console.BufferWidth / 2;
        Console.Write(new String('=', winWidth));
        Console.Write(new String('|', winWidth));
        foreach (var pane in _panels)
        {
            pane.Draw();
        }
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

        _panels[_activePanelIndex].HandleKey(info);
    }
}
