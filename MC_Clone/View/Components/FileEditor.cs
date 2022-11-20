using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class FileEditor : IComponent //TODO: Rename to FileViwer
{
    #region Atributes
    public Application Application { get; set; }

    private int _offset = 0;
    private int _selected = 0;
    private int _visible; //rows = Console.WindowWidth;
    private int maxWidth; //columns = Console.WindowWidth;

    private bool wrap = true;

    //cursor position
    private int _x = 0;
    private int _y = 0;

    public MyFileService FS;

    public FileSystemInfo File { get; set; }
    public List<string> OriginalRows = new List<string>();
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
        OriginalRows = FS.Read();
        Rows = OriginalRows;

        OnResize(); //first size
        Application.WinSize.OnWindowSizeChange += OnResize;
    }
    public void OnResize() //rename to: OnResize
    {
        Visible = Console.WindowHeight - 1 - 1; //-1 (Header) - 1 (Footer)
        maxWidth = Console.WindowWidth;

        Rows = FS.Read(); //OriginalRows //doesnt work -> why?
        Wrap();
    }

    public void Draw()
    {
        Console.SetCursorPosition(X,Y);
        for (int i = Offset; i < Offset + Math.Min(Visible, Rows.Count); i++)
        {
            //LineNumber(i); //debug //broken maxWidth -> bad Wrap

            Console.ForegroundColor = Config.Table_ForegroundColor;
            Console.BackgroundColor = Config.Table_BackgroundColor;

            Console.WriteLine(Rows[i]); //fix: console auto line wrap -> (destroys formatting)
        }
    }
    public void LineNumber(int i)
    {
        //selected is disabled
        if (i == Selected)
        {
            Console.ForegroundColor = Config.Table_Line_ACTIVE_ForegroundColor;
            Console.BackgroundColor = Config.Table_Line_ACTIVE_BackgroundColor;
        }
        Console.Write($"{i}. ");
    }
    public void Wrap()
    {
        for (int i = 0; i < Rows.Count; i++)
        {
            int locMaxWidth = maxWidth;// - 1;
            if (Rows[i].Length > maxWidth)
            {
                if (wrap)
                {
                    List<string> texts = new List<string>();
                    int lenght = Rows[i].Length;
                    int index = locMaxWidth;// - 1;
                    string text = "";
                    while (locMaxWidth + locMaxWidth < lenght) //+ index
                    {
                        text = Rows[i].Substring(index, locMaxWidth);
                        lenght -= locMaxWidth;
                        index += locMaxWidth - 1;
                        //Rows.Insert(i + 1, text);
                        texts.Add(text);
                    }
                    if (lenght < locMaxWidth + locMaxWidth)
                    {
                        text = Rows[i].Substring(index, lenght - locMaxWidth);
                        //Rows.Insert(i + 1, text);
                        texts.Add(text);
                        //throw new Exception(lenght.ToString());
                    }
                    texts.Reverse();
                    //Rows.InsertRange();
                    foreach (var item in texts)
                    {
                        Rows.Insert(i + 1, item);
                    }
                }
                Rows[i] = Rows[i].Substring(0, locMaxWidth);
            }
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
            case ConsoleKey.F5:
                GoTo(10);
                break;
        }
        Console.WriteLine(info.KeyChar);
    }

    #region Controls Methods
    private void ScrollUp()
    {
        Selected--;

        //if (Selected == Offset - 1)
            Offset--;
    }
    private void ScrollDown()
    {
        Selected++;

        //if (Selected == Offset + Math.Min(Visible, Rows.Count))
          Offset++;
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
    private void GoBegin()
    {
        Selected = 0;
        Offset = 0;
    }
    private void GoEnd()
    {
        Selected = Rows.Count - 1;
        Offset = Rows.Count - Visible;
    }

    private void GoTo(int goIndex)
    {
        Offset = goIndex;
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
