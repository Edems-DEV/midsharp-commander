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
            //if (FS_Objects.Count <= Visible) { return; }
            //if (value >= FS_Objects.Count - Visible) { _offset = FS_Objects.Count - Visible; _selected = FS_Objects.Count - 1; return; }
            _offset = value;
        }
    }
    public int Selected
    {
        get { return _selected; }
        set
        {
            if (value < 0) { return; }
            //if (value >= rows.Count - deadRows) { return; }
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
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            case ConsoleKey.UpArrow:
                Y--;
                return;
            case ConsoleKey.DownArrow:
                Y++;
                return;
            case ConsoleKey.LeftArrow:
                X--;
                return;
            case ConsoleKey.RightArrow:
                X++;
                return;
        }
        Console.WriteLine(info.KeyChar);
    }

    #region Controls Methods
    private void ScrollUp()
    {
        Selected--;

        if (Selected == Offset - 1)
            Offset--;
    }
    private void ScrollDown()
    {
        Selected++;

        //if (Selected == Offset + Math.Min(Visible, this.rows.Count))
            Offset++;
    }
    private void GoBegin()
    {
        Selected = 0;
        Offset = 0;
    }
    private void GoEnd()
    {
        //Selected = FS_Objects.Count - 1;
        //Offset = FS_Objects.Count - Visible;
    }
    private void PageUp()
    {
        Selected = Selected - Visible;
        Offset = Offset - Visible;
    }
    private void PageDown()
    {
        Selected = Selected + Visible;
        Offset = Offset + Visible;
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
