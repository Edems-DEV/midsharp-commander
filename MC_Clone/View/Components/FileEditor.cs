using System;
using System.Collections.Generic;
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

    //cursor position
    private int _x = 0;
    private int _y = 0;

    public MyFileService FS;
    public Cursor_2D Cursor;
    public Cursor_2D_Select Marker;

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
            case ConsoleKey.F2:
                SaveChanges();
                break;
            case ConsoleKey.F3:
                Mark();
                break;
            case ConsoleKey.F8:
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
        
        Marker.Hook(); //TODO: find better hook
    }

    public bool Marked = false;
    int start_X;
    int start_Y;
    
    int leftCursor_X = 0;
    int rightCursor_X = 0;
    int leftCursor_Y = 0;
    int rightCursor_Y = 0;
    List<string> selectedRows = new List<string>();

    public int linesCout { get { return(leftCursor_Y - rightCursor_Y); } } //+ 1
    public void Mark()
    {
        Marked = !Marked; 
        
        if (Marked) //save start cursor
        {
            //create - new object of Marker[select] (override old)
            if (Cursor.X_selected == start_X && Cursor.Y_selected == start_Y) //Cursor.pos = Marker.Pos => return (only cursor) [make pos object?]
            {
                return;
            }
            start_X = Cursor.X_selected;
            start_Y = Cursor.Y_selected;
        }
        else
        {
            //start_X = null; start_Y = null;
        }
    }
    public void MarkSetup()
    {
        //create object => contructor (move this things into him)
        SetCursorsSides();
        GetDataToMark();
        //DrawMarker();
    }
    public void SetCursorsSides() //SetCursorsSides
    {
        if (start_Y == Cursor.Y_selected)
        {
            if (start_X < Cursor.X_selected) //switch sides (left arrow)
            {
                leftCursor_X = start_X;
                rightCursor_X = Cursor.X_selected;
            }
            else
            {
                leftCursor_X = Cursor.X_selected;
                rightCursor_X = start_X;
            }
        }
        else if (start_Y < Cursor.Y_selected)
        {
            leftCursor_X = start_X;
            rightCursor_X = Cursor.X_selected;
        }
        else if (start_Y > Cursor.Y_selected)
        {
            leftCursor_X  = Cursor.X_selected;
            rightCursor_X = start_X;
        }
    }
    public void GetDataToMark() //move to class
    {
        int Y_counter = start_Y;
        
        if (Marked) //why this? (always is?)
        {
            if (start_Y == Cursor.Y_selected) //single row select
            {
                selectedRows.Add(Rows[Y_counter].Substring(leftCursor_X, rightCursor_X));
            }
            else
            {
                //start substring
                //full rows (if rows > 2)
                //end substring

                selectedRows.Add(Rows[Y_counter].Substring(leftCursor_X, Rows[Y_counter].Length - leftCursor_X)); //first line
                if (linesCout >= 2) //have middle full lines
                {
                    //save middle lines
                    while (Y_counter < linesCout - 2)
                    {
                        Y_counter++;
                        selectedRows.Add(Rows[Y_counter]);
                    }
                }
                selectedRows.Add(Rows[Y_counter].Substring(0, rightCursor_X)); //last row
            }
        }
    }
    public void DrawMarker()
    {
        int Y_counter = start_Y;

        if (Marked) //why this? (always is?)
        {
            ConsoleColor oldBG = Console.BackgroundColor;
            Console.BackgroundColor = Config.Accent_BackgroundColor;

            
            if (start_Y == Cursor.Y_selected) //single row select
            {
                DrawLineOn(Y_counter, leftCursor_X);
            }
            else
            {
                DrawLineOn(Y_counter, leftCursor_X);
                if (linesCout >= 2)
                {
                    while (Y_counter < linesCout - 2)
                    {
                        Y_counter++;
                        DrawLineOn(Y_counter);
                    }
                }
                DrawLineOn(Y_counter);
            }
            
            
            Console.BackgroundColor = oldBG;
        }
    }
    public void DrawLineOn(int rowIndex, int x = 0)
    {
        Console.SetCursorPosition(rowIndex, x);
        Console.Write(Rows[rowIndex]);
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