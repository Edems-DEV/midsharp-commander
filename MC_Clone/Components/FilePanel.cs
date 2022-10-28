using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class FilePanel : IComponent
{
    public event Action<int> RowSelected; 


    private List<string> headers;

    private List<Row> rows = new List<Row>();

    private int offset = 0;

    public int Selected { get; set; } = 0;

    public int Visible { get; set; } = 10;

    int x = 0;
    int y_temp = 0;
    int y = 0;

    //int lineLength;
    int maxNameLength = 20;
    private bool _active;

    public bool Active
    {
        get { return this._active; }
        set { this._active = value; }
    }

    public FilePanel(string[] headers, int X = 0, int Y = 0)
    {
        this.headers = new List<string>(headers);
        x = X;
        y = Y;
        y_temp = y;
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            //---------UPDATE---------
            case ConsoleKey.Tab:
                //ListWindow.ChangeActivePanel();
                break;
            case ConsoleKey.Enter:
                //ChangeDirectoryOrRunProcess();
                this.RowSelected(this.Selected);
                break;
            //---------NAVIGATION---------
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
            //------------------------
            //---------FOOTER---------
            //case ConsoleKey.F4:
            //    this.RowSelected(this.Selected);
            //    break;
            case ConsoleKey.F1:
                Help();
                break;
            case ConsoleKey.F2:
                Menu();
                break;
            case ConsoleKey.F3:
                View();
                break;
            case ConsoleKey.F4:
                Edit();
                break;
            case ConsoleKey.F5:
                Copy();
                break;
            case ConsoleKey.F6:
                RenMov();
                break;
            case ConsoleKey.F7:
                MkDir();
                break;
            case ConsoleKey.F8:
                MkDir();
                break;
            case ConsoleKey.F9:
                PullDn();
                break;
            case ConsoleKey.F10:
                MkDir();
                break;
            case ConsoleKey.Escape:
                this.Quit();
                break;
        }
    }

    public void Draw()
    {
        List<int> widths = Widths();

        DrawData(null, widths, '┬', '─', '┌', '┐');
        DrawData(headers, widths, '│', ' ');
        DrawData(null, widths, '┼', '─', '├', '┤');

        for (int i = offset; i < offset + Math.Min(Visible, this.rows.Count); i++)
        {
            if (i == Selected)
            {
                Console.ForegroundColor = Config.FocusText;
                Console.BackgroundColor = Config.FocusBackgroud;
            }
            
            DrawData(rows[i].Data, widths, '│', ' ');

            Console.ResetColor();
        }

        DrawData(null, widths, '┴', '─', '└', '┘');
        y_temp = y;
    }
    #region Draw methods
    private void DrawData(List<string>? data, List<int> widths, char sep, char pad, char start = 'ĉ', char end = 'ĉ')
    {
        char _start = start == 'ĉ' ? sep : start;
        char _end = end == 'ĉ' ? sep : end;
        
        int i = 0;
        //y = Console.CursorTop;
        Console.SetCursorPosition(x,y_temp);
        y_temp++;
        foreach (int width in widths)
        {
            string value = data != null ? data[i] : "";
            char b = i == 0 ? _start : sep;

            Console.Write(b);
            Console.Write(pad);
            Console.Write(value.PadRight(widths[i] + 1, pad));

            i++;
        }

        Console.WriteLine(_end);
    }

    public int LineLength()
    {
        int lenght = 0;
        foreach (var item in Widths())
        {
            lenght += item;
            lenght += 2;
        }
        Console.SetCursorPosition(x, 1);
        Console.WriteLine(lenght);
        return lenght;
    }
    private List<int> Widths()
    {
        List<int> widths = new List<int>();

        for (int i = 0; i < headers.Count; i++)
        {
            int width = headers[i].Length;

            foreach (Row item in rows)
            {
                if (item.Data[i].Length > width)
                    width = item.Data[i].Length;
            }

            widths.Add(width);
        }

        return widths;
    }
    public void Add(string[] data)
    {
        if (data.Length != headers.Count)
            throw new ArgumentException("Invalid columns count");

        rows.Add(new Row(data));
    }

    public void ImportRows(string path)
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
            Add(new string[] { @"\" + Truncated(di.Name, maxNameLength - 1), size.ToString(), di.LastWriteTime.ToString("MMM dd HH:mm") });
        }
        //Files
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            FileInfo fi = new FileInfo(file);
            Add(new string[] { Truncated(fi.Name, maxNameLength), fi.Length.ToString(), fi.LastWriteTime.ToString("MMM dd HH:mm") });
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

    #endregion

    #region HandleKey methods
    #region Controls
    private void ScrollUp()
    {
        if (Selected <= 0)
            return;

        Selected--;

        if (Selected == offset - 1)
            offset--;
    }
    private void ScrollDown()
    {
        if (Selected >= rows.Count - 1)
            return;

        Selected++;

        if (Selected == offset + Math.Min(Visible, this.rows.Count))
            offset++;
    }
    private void GoBegin()
    {
        Selected = 0;
        offset = 0;
    }
    private void GoEnd()
    {
        Selected = rows.Count - 1;
        offset = rows.Count - Visible;
    }
    private void PageUp()
    {
        //TODO: Opravit podmíky - (out of range)
        //if (offset > Visible)
        //    return;

        Selected = Selected - Visible;
        offset = offset - Visible;
    }
    private void PageDown()
    {
        //TODO: Opravit podmíky - (out of range)
        //if (rows.Count < offset + Visible)
        //    return;

        Selected = Selected + Visible;
        offset = offset + Visible;
    }
    #endregion
    #region FunctionKeys
    private void Help()
    {
    }
    private void Menu()
    {
    }
    private void View()
    {
    }
    private void Edit()
    {
    }
    private void Copy()
    {
    }
    private void RenMov()
    {
    }
    private void MkDir()
    {
    }
    private void Delete()
    {
    }
    private void PullDn()
    {
    }
    private void Quit()
    {
    }
    #endregion
    #endregion
}
