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
    Footer footer;
    MenuBar MenuBar;

    static void Y_autoSize_IDEA() //delMe
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

        int paneY = 1;
        FilePanel pane1 = new FilePanel(Config.Path_LeftPane, 0, paneY);
        FilePanel pane2 = new FilePanel(Config.Path_RightPane, winWidth, paneY);
        this._panels.Add(pane1);
        this._panels.Add(pane2);
        
        string[] labels = { "Drives", "MkFile", "View", "Edit", "Copy", "RenMov", "Mkdir", "Delete", "PullDn", "Quit" };
        footer = new Footer(labels);
        MenuBar = new MenuBar();
        
        foreach (var pane in _panels)
        {
            pane.ImportRows(pane.Path_); //useless?
            pane.RowSelected += Table_RowSelected;

        }
        _activePanelIndex = 0;
        ActivePanel().IsActive = true;
        KeyPress += ActivePanel().HandleKey;
    }

    public void ChangeActivePanel()
    {
        ActivePanel().IsActive = false;
        KeyPress -= ActivePanel().HandleKey;

        this._activePanelIndex++;
        if (this._activePanelIndex >= this._panels.Count)
        {
            this._activePanelIndex = 0;
        }

        ActivePanel().IsActive = true;
        KeyPress += ActivePanel().HandleKey;
    }

    private void Table_RowSelected(int index)
    {
        this.Application.SwitchWindow(new EditWindow(index));
    }

    public override void Draw()
    {
        MenuBar.Draw();
        
        foreach (var pane in _panels)
        {
            pane.Draw();
        }

        footer.Draw(); // y - 1 (from bottom)
    }

    public override void HandleKey(ConsoleKeyInfo info)
    {
        if (info.Key == ConsoleKey.Tab)
        {
            ChangeActivePanel();
        }

        ActivePanel().HandleKey(info);
    }
}
