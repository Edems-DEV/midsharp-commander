using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MC_Clone;

public class FilePanel : IComponent
{
    public event Action<int> RowSelected;


    private List<string> headers = new List<string>(new string[] { "Name", "Size", "Date" });

    private List<Row> rows = new List<Row>();
    private List<FileSystemInfo> FS_Objects = new List<FileSystemInfo>();

    FileManager FM;

    private int offset = 0;

    public int Selected { get; set; } = 0;

    public int Visible { get; set; } = 10;

    int x = 0;
    int y_temp = 0;
    int y = 0;

    int lineLength = 0;
    int maxNameLength = 20; // lineLength - 26(Size + Date + |) - 1(first '|')
    int NameExtraPad = 0;
    private bool _active;
    private bool _discs;
    private string _path = "";

    string statusBarLabel = "";

    #region Static
    char folderPrefix = '/';
    int halfWindow = 0;
    int deadRows = 0;
    #endregion


    #region Properties
    public bool Active
    {
        get { return this._active; }
        set { this._active = value; }
    }
    public bool IsDiscs
    {
        get { return this._discs; }
    }
    public string Path_
    {
        get { return this._path; }
        set
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(value);
            if (directoryInfo.Exists)
                this._path = value;
            else
                throw new Exception(String.Format("Path {0} does not exist", value));
        }
    }
    #endregion

    #region Model Wrappers
    private void SetDiscs()
    {
        _discs = true;
        FS_Objects = FM.SetDiscs();
    }
    private void SetLists(string path)
    {
        _discs = false;
        FS_Objects = FM.SetLists(path);
    }
    
    private void ChangeDir()
    {
        Path_ = FM.ChangeDir(Path_, GetActiveObject());
        RefreshPanel();
    }
    #endregion


    #region Constructor
    void Start(int X = 0, int Y = 0)
    {
        x = X;
        y = Y;
        y_temp = y;
        LineLength();
        halfWindow = Console.WindowWidth / 2;
        FM = new FileManager();

    }
    public FilePanel(int X = 0, int Y = 0)
    {
        Start(X, Y);
        SetDiscs();
    }

    public FilePanel(string path, int X = 0, int Y = 0)
    {
        Path_ = path;
        Start(X, Y);
        if (path == ".") { SetDiscs(); return; }
        SetLists(Path_);
    }
    #endregion

    public void HandleKey(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            //---------UPDATE---------
            case ConsoleKey.Enter:
                ChangeDir();
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
            case ConsoleKey.F1:
                Drives();
                break;
            case ConsoleKey.F2:
                CreateFile();
                break;
            case ConsoleKey.F3:
                if (GetActiveObject() is DirectoryInfo) 
                    { ChangeDir(); ; break; }
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
                Delete();
                break;
            case ConsoleKey.F9:
                //MenuBar.active = true;
                //PullDn();
                break;
            case ConsoleKey.F10:
            case ConsoleKey.Escape:
                this.Quit();
                break;
        }
    }
    
    private string[] Generete_StatusLine(string label)
    {
        string row0 = "";
        row0 += l.upRight;
        row0 += new String(l.lineX, lineLength - 2);
        row0 += l.upleft;
        string row1 = "";
        row1 += l.lineY;
        if (label == folderPrefix + "..")
            label = "UP--DIR";
        else if (!label.StartsWith(folderPrefix))
            label = " " + label;
        row1 += label; //modify for each file type
        row1 += new String(' ', lineLength - row1.Length - 1);
        row1 += l.lineY;
        string row2 = "";
        row2 += l.bottomLeft;
        row2 += new String(l.lineX, lineLength - 2);
        row2 += l.bottomRight;
        string[] local_rows = { row0, row1, row2 };

        return local_rows;
    }
    public void StatusLine(string label) //char vertical = l.lineY //new element?
    {
        string[] local_rows = Generete_StatusLine(label);

        int count = 0;
        foreach (var local_row in local_rows)
        {
            Console.SetCursorPosition(x, y_temp + count);
            Console.Write(local_row);
            count++;
        }
        string diskInfo = FM.freeSpace();
        int currentLeftCursor = Console.CursorLeft;
        if (currentLeftCursor > (diskInfo.Length + 2))
            Console.CursorLeft = currentLeftCursor - (diskInfo.Length + 2);
        Console.Write(diskInfo);

    }

   
    void activePath()
    {
        ConsoleColor oldTextColor = Console.ForegroundColor;
        ConsoleColor oldBackColor = Console.BackgroundColor;


        Console.SetCursorPosition(x, y_temp - 1);

        string line = @$"{l.topLeft}{l.arrowRight}{l.lineX}"; //@$"┌<─";
        string label = "";

        Console.Write(line);
        if (_active) {
            Console.ForegroundColor = Config.Table_Path_ACTIVE_ForegroundColor;
            Console.BackgroundColor = Config.Table_Path_ACTIVE_BackgroundColor;
        }
        if (_discs)
            label = " Drives: ";
        else
            label = $" {_path} ";
        Console.Write(label);
        //Console.ResetColor();
        Console.ForegroundColor = oldTextColor;
        Console.BackgroundColor = oldBackColor;
    }

    public void Draw()
    {
        ImportRows(); //update long row (laggy)
        List<int> widths = Widths();
        LineLength();
        Visible = Console.WindowHeight - 1 - 1 - 2 - 3 -1; //-1 (Menu) - 3 (Header) - 3 (Status + FKey) - 1 (fKey ofset)
        //var a = new Logs(Visible.ToString());
        halfWindow = Console.WindowWidth / 2;

        if (x != 0)
        {
            x = halfWindow;
        }

        Console.ForegroundColor = Config.Table_ForegroundColor;
        Console.BackgroundColor = Config.Table_BackgroundColor;

        DrawData(null, widths, l.down, l.lineX, l.topLeft, l.topRight);
        activePath();
        DrawData(headers, widths, l.lineY, ' ');
        DrawData(null, widths, l.cross, l.lineX, l.upRight, l.upleft);

        for (int i = offset; i < offset + Math.Min(Visible, this.rows.Count); i++)
        {
            if (i == Selected)
            {
                Console.ForegroundColor = Config.Table_Line_ACTIVE_ForegroundColor;
                Console.BackgroundColor = Config.Table_Line_ACTIVE_BackgroundColor;
                statusBarLabel = rows[i].Data[0];
            }
            
            DrawData(rows[i].Data, widths, l.lineY, ' ');

            //Console.ResetColor();
            Console.ForegroundColor = Config.Table_ForegroundColor;
            Console.BackgroundColor = Config.Table_BackgroundColor;
        }

        //DrawData(null, widths, l.up, l.lineX, l.bottomLeft, l.bottomRight);
        StatusLine(statusBarLabel);
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

    #region Calc
    public int LineLength() //TODO: refactor
    {
        int lenght = 0;
        foreach (var item in Widths())
        {
            lenght += 2;
            lenght += item;
            lenght += 1;
        }
        lenght += 1;
        int HalfWIn = Console.BufferWidth / 2;
        if (lenght > 27)
        {
            maxNameLength = HalfWIn - 26 - 1 - 2; // lineLength - 26(Size + Date + |) - 1(first '|')
        }
        lineLength = lenght;
        Console.SetCursorPosition(x, 1);
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
    #endregion

    #region Prepere data
    public void Add(string[] data)
    {
        if (data.Length != headers.Count)
            throw new ArgumentException("Invalid columns count");

        rows.Add(new Row(data));
    }

    public void ImportRows(string path = "")
    {
        deadRows = 0;
        int aaaa = 5;
        if (halfWindow >= 29) //need to update
        {
            aaaa = ((halfWindow) - 27 - 2);
        }

        if (rows != null)
            rows.Clear();
        foreach (var item in FS_Objects)
        {
            if (item == null)
            {
                Add(new string[] { folderPrefix + "..", "UP--DIR", "ToDo" });
                continue;
            }
            string name = Truncate.Text(item.Name, aaaa);
            int local_maxNameLength = maxNameLength;

            long size = 0;
            if (item is DirectoryInfo)
            {
                if (!_discs)
                    name = folderPrefix + name;
                local_maxNameLength -= 1;
                DirectoryInfo a = item as DirectoryInfo;
                try { //missing permision for low level folders
                    foreach (var file in a.GetFiles()) //TODO: SubDir size
                    {
                        size += file.Length;
                    }
                }
                catch { }
                
            }
            else
            {
                FileInfo a = item as FileInfo;
                size = a.Length;
            }
            
            Truncate.Text(item.Name, local_maxNameLength);
            Add(new string[] { name, Truncate.Size(size).PadLeft(7), item.LastWriteTime.ToString("MMM dd HH:mm") });
        }
        //Long row
        //TODO: find different solution
        string emptyLong = new String(' ', aaaa);
        int asss = rows.Count + 8;
        for (int i = 0; i < Console.WindowHeight - asss; i++)
        {
            Add(new string[] { emptyLong, "", "            " });
            deadRows++;
        }
    }
    #endregion


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
        if (Selected >= rows.Count - 1 - deadRows)
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
        int rowsCout = rows.Count;
        if (deadRows > 0 && deadRows < rows.Count)
        {
            rowsCout -= deadRows;

            if (rowsCout - 1 < 0 && rowsCout - Visible < 0)
            {
                rowsCout = rows.Count;
            }
        }
        Selected = rowsCout - 1;
        //offset = rowsCout - Visible;

        //Selected = rows.Count - 1;
        offset = rows.Count - Visible; //dead Rows never exist when offset is
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
    private void Drives() //Help
    {
        SetDiscs();
        ImportRows();
        Draw();
    }
    private void CreateFile() //Menu //TODO: move
    {
        if (IsDiscs)
            return;
        string destPath = Path_;
        string fileName = this.AksName("Enter the file name: "); //TODO: change to popUp
        if (!fileName.Contains('.'))
        {
            fileName = fileName + ".txt";
        }
        try
        {
            string fileFullPath = Path_ + @"\" +  fileName;
            DirectoryInfo dir = new DirectoryInfo(fileFullPath);
            if (!File.Exists(fileFullPath))
            {
                using (FileStream fs = File.Create(fileFullPath));
            }
                //dir.Create();
            else
                this.ShowMessage("A catalog with that name already exists");
            SetLists(Path_);
            ImportRows();
            this.Draw();
        }
        catch (Exception e)
        {
            this.ShowMessage(e.Message);
        }
    }
    private void View()
    {
    }
    private void Edit()
    {
        this.RowSelected(this.Selected);
    }
    private void Copy()
    {
        string destPath = this.AksName("Enter the catalog name: "); //TODO: change to popUp
        try
        {
            FileSystemInfo fileObject = GetActiveObject();
            FileInfo currentFile = fileObject as FileInfo;
            if (currentFile != null)
            {
                string fileName = currentFile.Name;
                string destName = Path.Combine(destPath, fileName);
                File.Copy(currentFile.FullName, destName, true);
            }
            else
            {
                string currentDir = ((DirectoryInfo)fileObject).FullName;
                string destDir = Path.Combine(destPath, ((DirectoryInfo)fileObject).Name);
                CopyDirectory(currentDir, destDir);
            }
            SetLists(Path_);
            ImportRows();
            this.Draw();
        }
        catch (Exception e)
        {
            this.ShowMessage(e.Message);
            return;
        }
    }
    private void CopyDirectory(string sourceDirName, string destDirName) //TODO: change to popUp
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();
        if (!Directory.Exists(destDirName))
            Directory.CreateDirectory(destDirName);
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, true);
        }
        foreach (DirectoryInfo subdir in dirs)
        {
            string temppath = Path.Combine(destDirName, subdir.Name);
            CopyDirectory(subdir.FullName, temppath);
        }
    }
    private void RenMov()
    {
        string destPath = this.AksName("Enter the catalog name: "); //TODO: change to popUp
        try
        {
            FileSystemInfo fileObject = GetActiveObject();
            string objectName = fileObject.Name;
            string destName = Path.Combine(destPath, objectName);
            if (fileObject is FileInfo)
                ((FileInfo)fileObject).MoveTo(destName);
            else
                ((DirectoryInfo)fileObject).MoveTo(destName);
            SetLists(Path_);
            ImportRows();
            this.Draw();
        }
        catch (Exception e)
        {
            this.ShowMessage(e.Message);
            return;
        }
    }
    private void MkDir()
    {
        if (IsDiscs)
            return;
        string destPath = Path_;
        string dirName = this.AksName("Enter the folder name: "); //TODO: change to popUp
        try
        {
            string dirFullName = Path.Combine(destPath, dirName);
            DirectoryInfo dir = new DirectoryInfo(dirFullName);
            if (!dir.Exists)
                dir.Create();
            else
                this.ShowMessage("A catalog with that name already exists");
            SetLists(Path_);
            ImportRows();
            this.Draw();
        }
        catch (Exception e)
        {
            this.ShowMessage(e.Message);
        }
    }

    private string AksName(string message)
    {
        string name;
        Console.CursorVisible = true;
        do
        {
            this.ShowMessage(message);
            name = Console.ReadLine();
        } while (name.Length == 0);
        Console.CursorVisible = false;
        return name;
    }

    private void ShowMessage(string message)
    {
        PrintString(message, 0, Console.WindowHeight -2, Config.MsgBoxForegroundColor, Config.MsgBoxBackgroundColor);
    }

    public static void PrintString(string str, int X, int Y, ConsoleColor text, ConsoleColor background)
    {
        Console.ForegroundColor = text;
        Console.BackgroundColor = background;

        Console.SetCursorPosition(X, Y);
        Console.Write(str);

        Console.ForegroundColor = Config.MsgBoxForegroundColor;
        Console.BackgroundColor = Config.MsgBoxBackgroundColor;
    }
    private void Delete()
    {
        if (IsDiscs)
            return;
        FileSystemInfo fileObject = GetActiveObject();
        try
        {
            if (fileObject is DirectoryInfo)
                ((DirectoryInfo)fileObject).Delete(true);
            else
                ((FileInfo)fileObject).Delete();
            SetLists(Path_);
            ImportRows();
            Selected -= 1;
            this.Draw();
        }
        catch (Exception e)
        {
            //this.ShowMessage(e.Message);
            return;
        }
    }
    private void PullDn()
    {
    }
    private void Quit()
    {
        Console.Clear();
        Environment.Exit(0);
    }
    #endregion
    #endregion




    #region Misc
    public FileSystemInfo GetActiveObject()
    {
        if (FS_Objects != null && FS_Objects.Count != 0)
        {
            return FS_Objects[Selected];
        }
        throw new Exception("The list of panel objects is empty");
    }

    private void RefreshPanel()
    {
        offset = 0;
        Selected = 0;
        UpdatePanel();
    }
    private void UpdatePanel()
    {
        Clear(); //change to something better (clear only that pane)
        ImportRows();
        Draw();
    }


    private void Clear()
    {
        int rows = Visible + headers.Count + 1 + 2; //final line + 2 Menu

        string space = new String(' ', lineLength);
        for (int i = 0; i < rows; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.WriteLine(space);
        }
    }
    #endregion
}
