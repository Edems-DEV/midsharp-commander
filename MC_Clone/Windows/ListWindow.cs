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
    

    public ListWindow()
    {
        FilesService service = new FilesService(Config.FILE);
        string[] staticHeader = new string[] { "Name", "Size", "Date" }; //chage me

        this.pane1 = new FilePanel(staticHeader, 0, 4); //service.Headers()
        this.pane2 = new FilePanel(staticHeader, 55, 4); //table.LineLength() + 2 //55
        this._panels.Add(pane1);
        this._panels.Add(pane2);

        foreach (var pane in _panels)
        {
            pane.ImportRows(Config.FOLDER);
            pane.RowSelected += Table_RowSelected;

        }
        _activePanelIndex = 0;
        this._panels[this._activePanelIndex].Active = true;
        KeyPress += this._panels[this._activePanelIndex].HandleKey;
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

        _panels[_activePanelIndex].HandleKey(info);
    }
}
