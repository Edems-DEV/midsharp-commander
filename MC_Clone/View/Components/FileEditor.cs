using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class FileEditor : IComponent
{
    #region Atributes
    public Application Application { get; set; }

    private int _offset = 0;
    private int _selected = 0;
    private int _visible; //rows = Console.WindowWidth;
    private int maxWidth; //columns = Console.WindowWidth;

    //cursor position
    private int _x = 0;
    private int _y = 0;

    public MyFileService FS;

    public FileSystemInfo File { get; set; }
    public List<string> Rows = new List<string>(); //real rows in txt
    public List<string> PrintRows = new List<string>(); //rows wraped as needed -> works with 'visible'

    #endregion

    #region Properties
    //ToDo: new object 'Cursor'
    public int Y
    {
        get { return _y; }
        set
        {
            if (value < 0)
                throw new Exception(String.Format($"Y < 0 (val: {value})"));
            _y = value;
        }
    }

    public int X
    {
        get { return _x; }
        set
        {
            if (value < 0)
                throw new Exception(String.Format($"X < 0 (val: {value})"));
            _x = value;
        }
    }

    public int Visible
    {
        get { return _visible; }
        set
        {
            if (value < 0)
                throw new Exception(String.Format($"Visible < 0 (val: {value})"));
            _visible = value;
        }
    }
    public int Offset
    {
        get { return _offset; }
        set
        {
            if (value < 0) { _offset = 0; return; }
            if (Rows.Count <= Visible) { return; }
            if (value >= Rows.Count - Visible) { _offset = Rows.Count - Visible; _selected = Rows.Count - 1; return; }
            _offset = value;
        }
    }
    public int Selected
    {
        get { return _selected; }
        set
        {
            if (value < 0) { return; }
            if (value >= Rows.Count) { return; }
            _selected = value;
        }
    }
    #endregion
    public FileEditor(Application application, FileInfo file, int x = 0, int y = 0)
    {
        this.Application = application;
        this.File = file;
        this.X = x;
        this.Y = y;
        FS = new MyFileService(file.FullName);
        Rows = FS.Read();

        OnResize(); //first size
        Application.WinSize.OnWindowSizeChange += OnResize;
    }
    public void OnResize() //rename to: OnResize
    {
        Visible = Console.WindowHeight - 1 - 1; //-1 (Header) - 1 (Footer)
        maxWidth = Console.WindowWidth;
    }

    public void Draw()
    {
        for (int i = Offset; i < Offset + Math.Min(Visible, Rows.Count); i++)
        {
            if (i == Selected)
            {
                Console.ForegroundColor = Config.Table_Line_ACTIVE_ForegroundColor;
                Console.BackgroundColor = Config.Table_Line_ACTIVE_BackgroundColor;
            }
            Console.Write($"{i}. ");
            Console.ForegroundColor = Config.Table_ForegroundColor;
            Console.BackgroundColor = Config.Table_BackgroundColor;

            Console.WriteLine(Rows[i]);
        }
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            case ConsoleKey.UpArrow:
                ScrollUp();
                break;
            case ConsoleKey.DownArrow:
                ScrollDown();
                break;
            case ConsoleKey.Home:
                GoBegin();
                break;
            case ConsoleKey.End:
                GoEnd();
                break;
            case ConsoleKey.PageUp:
                PageUp();
                break;
            case ConsoleKey.PageDown:
                PageDown();
                break;
        }
        Console.WriteLine(info.KeyChar);
    }

    #region Controls Methods
    private void ScrollUp()
    {
        //Selected--;

        //if (Selected == Offset - 1)
            Offset--;
    }
    private void ScrollDown()
    {
        //Selected++;

        //if (Selected == Offset + Math.Min(Visible, Rows.Count))
            Offset++;
    }
    private void PageUp()
    {
        //Selected = Selected - Visible;
        Offset = Offset - Visible;
    }
    private void PageDown()
    {
        //Selected = Selected + Visible;
        Offset = Offset + Visible;
    }
    private void GoBegin()
    {
        //Selected = 0;
        Offset = 0;
    }
    private void GoEnd()
    {
        //Selected = Rows.Count - 1;
        Offset = Rows.Count - Visible;
    }

    private void GoTo()
    {
        int GoTo = 10;
        Selected = Rows.Count - 1;
    }

    #endregion

    #region FKeys methods
    public void WrapLine(bool state) //bool outside? to stay persistent
    {
    }
    public void Hex_Asci() { }
    public void GoToLine()
    {

    }
    public void SearchText()
    {

    }
    #endregion

    private void Write(ConsoleKeyInfo info)
    {
        Console.WriteLine(info.KeyChar);
    }
}
