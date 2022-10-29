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
    public string logMSG = "";

    static void Test()
    {
        //Elements Y size
        int menu = 1;
        int PANES;
        int header = 3; int lasLine = 1;
        PANES = header + lasLine;
        int miniStatusBar = 2;
        int DebugConsole = 1;
        int footer = 1;

        //Top
        int winHeight = Console.WindowHeight;
        int cursorFromTop = 0;
        int cursorFromBottom = winHeight;

        //Size
        cursorFromTop += menu;
        cursorFromTop += header;
       
        cursorFromBottom -= footer;
        cursorFromBottom -= DebugConsole;
        cursorFromBottom -= miniStatusBar;
        cursorFromBottom -= lasLine;
        int middleSpace = winHeight - (cursorFromTop + cursorFromBottom);

        //Panel(X,Y)
        int y = cursorFromTop;
        int visible = middleSpace;
    }
    
    FilePanel ActivePanel()
    {
        return _panels[_activePanelIndex];
    }

    public ListWindow()
    {
        winWidth = Console.BufferWidth / 2;
        FilesService service = new FilesService(Config.FILE);
        
        FilePanel pane1 = new FilePanel(Config.FILE, 0, 4); //service.Headers()
        FilePanel pane2 = new FilePanel(Config.FILE, winWidth, 4); //table.LineLength() + 2 //55
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
        winWidth = Console.WindowWidth / 2;
        Console.Write(new String('=', winWidth));
        Console.Write(new String('|', winWidth));
        foreach (var pane in _panels)
        {
            pane.Draw();
        }
        var dC = new DebugConsole(1);
        dC.Log(logMSG);
        dC.Draw();
        string[] labels = { "Help", "Menu", "View", "Edit", "Copy", "RenMov", "Mkdir", "Delete", "PullDn", "Quit" };
        var footer = new Footer(labels);
        footer.Draw(); // y - 1 (from bottom)
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
