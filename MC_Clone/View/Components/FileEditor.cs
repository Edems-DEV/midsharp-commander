using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class FileEditor : IComponent
{
    #region Atributes
    public Application Application { get; set; }

    //private int _visible; // | rows = Console.WindowWidth;
    //private int maxWidth; // - columns = Console.WindowWidth;

    //Cursor position
    private int _x = 0;
    private int _y = 0;

    public MyFileService FS;
    public Cursor_2D Cursor;
    public Cursor_2D_Select Marker;
    public Cursor_2D_FindSelect Select;

    public FileSystemInfo File { get; set; }
    public List<string> OriginalRows = new List<string>();
    public List<string> Rows = new List<string>(); //real rows in txt
    public List<string> PrintRows = new List<string>(); //rows wraped as needed -> works with 'visible'

    public string flag = "-";

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

    public string ActiveRow
    {
        get { return Rows[Cursor.Y_selected]; }
        set {
            Cursor.Row = value;
            Rows[Cursor.Y_selected] = value;
        }
    }
    #endregion

    private void Start(Application application, FileInfo file, int x = 0, int y = 0)
    {
        this.Application = application;
        this.File = file;
        this.X = x;
        this.Y = y;
        FS = new MyFileService(file.FullName);
        OriginalRows = FS.Read();
        Rows = new List<string>(OriginalRows);
        PrintRows = new List<string>(OriginalRows);
        Cursor = new Cursor_2D(Y, 0, Rows.Count, X, 0);
        Marker = new Cursor_2D_Select(this);
        Select = new Cursor_2D_FindSelect(this);

        OnResize(); //first size
        Application.WinSize.OnWindowSizeChange += OnResize;
    }
    public FileEditor(Application application, FileInfo file, int x = 0, int y = 0)
    {
        Start(application, file, x, y);
    }
    public void OnResize() //rename to: OnResize
    {
        Rows = new List<string>(OriginalRows);
        Wrap();
        Cursor.Y_visible = Console.WindowHeight - 1 - 1; //-1 (Header) - 1 (Footer)
        Cursor.X_visible = Console.WindowWidth;
    }

    public void Draw()
    {
        Wrap();
        Console.SetCursorPosition(X, Y);
        for (int i = Cursor.Y_offset; i < Cursor.Y_offset + Math.Min(Cursor.Y_visible, Rows.Count); i++)
        {
            if (i == Cursor.Y_selected)
            {
                Cursor.Row = Rows[i];
                Rows[i] = Cursor.Row;
            }

            Console.ForegroundColor = Config.Table_ForegroundColor;
            Console.BackgroundColor = Config.Table_BackgroundColor;

            Console.WriteLine(PrintRows[i]); //fix: console auto line wrap -> (destroys formatting)

            Select.Draw();
            Marker.Hook(); //TODO: find better hook
            Cursor.Draw();
        }
    }

    public string NextRow()
    {
        if (Cursor.Y_selected == Cursor.Y_totalSize - 1)
            return ActiveRow;

        return Rows[Cursor.Y_selected + 1];
    }
    public string PrevioustRow()
    {
        if (Cursor.Y_selected == 0)
            return ActiveRow;

        return Rows[Cursor.Y_selected - 1];
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            //Controls
            case ConsoleKey.RightArrow:
                Cursor.Right(NextRow());
                break;
            case ConsoleKey.LeftArrow:
                Cursor.Left(PrevioustRow());
                break;
            //---
            case ConsoleKey.UpArrow:
                Cursor.Up(PrevioustRow());
                break;
            case ConsoleKey.DownArrow:
                Cursor.Down(NextRow());
                break;
            case ConsoleKey.Home:
                Cursor.GoBegin();
                break;
            case ConsoleKey.End:
                Cursor.GoEnd();
                break;
            case ConsoleKey.PageUp:
                Cursor.PageUp();
                break;
            case ConsoleKey.PageDown:
                Cursor.PageDown();
                break;
            //Edit
            case ConsoleKey.Delete:
                Delete();
                break;
            case ConsoleKey.Backspace:
                Backspace();
                break;
            case ConsoleKey.Enter:
                Enter();
                break;
            //FKeys
            case ConsoleKey.F1:
                Marker.DebugPrint();
                break;
            case ConsoleKey.F2:
                SaveChanges();
                break;
            case ConsoleKey.F3:
                Marker.Mark();
                break;
            case ConsoleKey.F4:
                Select.Replace();
                break;
            case ConsoleKey.F5:
                Marker.Copy();
                break;
            case ConsoleKey.F6:
                Marker.Move();
                break;
            case ConsoleKey.F7:
                Select.Search();
                break;
            case ConsoleKey.F8:
                if (Marker.SelectionAlive)
                    Marker.Delete();
                else
                    DeleteLine();
                break;
            case ConsoleKey.Escape:
            case ConsoleKey.F10:
                Quit();
                break;
            default:
                WriteChar(info.KeyChar); //TODO: check for bad chars
                break;
        }
        
        //Marker.Hook(); //TODO: find better hook
    }
    
    public bool ContentChanged() //change to return  true if changed false = same
    {
        var set = new HashSet<string>(OriginalRows);
        return set.SetEquals(Rows);
    }
    public void Quit()
    {
        if (!ContentChanged())
            Application.SwitchPopUp(new CloseFile(File, Rows));
        else
            Application.SwitchWindow(new ListWindow(Application));
    }
    public void SaveChanges()
    {
        Application.SwitchPopUp(new SaveFile(File, Rows));
    }
    public void DeleteLine()
    {
        Rows.RemoveAt(Cursor.Y_selected);
        Cursor.Y_selected--;
    }

    public void Wrap()
    {
        int a = Console.WindowWidth;
        for (int i = 0; i < Rows.Count; i++)
        {
            int locMaxWidth = a;// - 1;
            if (Rows[i].Length > a)
            {
                int lastLenght = Rows[i].Length - Cursor.X_offset;
                if (lastLenght < locMaxWidth)
                    locMaxWidth = lastLenght;
                PrintRows[i] = Rows[i].Substring(Cursor.X_offset, locMaxWidth);
            }
            else
            {
                if (Rows[i].Length <= Cursor.X_offset)
                {
                    PrintRows[i] = " ";
                }
                else
                {
                    PrintRows[i] = Rows[i];
                }
            }
        }
    }
    public void Enter()
    {
        int CursorPos = Cursor.X_offset + Cursor.X_selected;
        string newLine = "";
        newLine = ActiveRow.Substring(CursorPos, ActiveRow.Length - CursorPos);
        if (newLine == "" || newLine == null){newLine = " ";}
        Rows.Insert(Cursor.Y_selected + 1, newLine);
        PrintRows = new List<string>(Rows);

        ActiveRow = ActiveRow.Substring(0, CursorPos);
        Cursor.X_selected = 0;
        Cursor.Y_selected += 1;
    }
    public void WriteChar(char Input)
    {
        int CursorPos = Cursor.X_offset + Cursor.X_selected;
        ActiveRow =
            ActiveRow.Substring(0, CursorPos)
            + Input + 
            ActiveRow.Substring(CursorPos, ActiveRow.Length - CursorPos);
        Cursor.X_selected++;
    }

    public void Backspace()
    {
        int CursorPos = Cursor.X_offset + Cursor.X_selected;
        if (CursorPos == 0)
        {
            string deletedRow = ActiveRow;
            DeleteLine();
            Cursor.X_selected = ActiveRow.Length;
            ActiveRow = ActiveRow + deletedRow;
        }
        else if (CursorPos == ActiveRow.Length - 1)
        {
            Cursor.X_selected--;
            ActiveRow = ActiveRow.Remove(ActiveRow.Length - 1);
        }
        else
        {
            ActiveRow =
            ActiveRow.Substring(0, CursorPos - 1)
            +
            ActiveRow.Substring(CursorPos, ActiveRow.Length - CursorPos); //-1

            Cursor.X_selected--;
        }
    }
    public void Delete()
    {
        int CursorPos = Cursor.X_offset + Cursor.X_selected;
        ActiveRow =
            ActiveRow.Substring(0, CursorPos)
            +
            ActiveRow.Substring(CursorPos + 1, ActiveRow.Length - 1 - CursorPos);
    }
}