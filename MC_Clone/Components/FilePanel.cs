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

    FileManager FM;

    private List<Row> rows = new List<Row>();
    private List<FileSystemInfo> FS_Objects = new List<FileSystemInfo>();
    private List<string> headers = new List<string>(new string[] { "Name", "Size", "Date" });

    #region Atributes
    private int x  = 0;
    private int y  = 0;
    private int y_temp  = 0;

    private int deadRows = 0;
    private int lineLength = 0;
    private int halfWindowSize = 0;
    private int maxNameLength = 20; // lineLength - 26(Size + Date + |) - 1(first '|')
    private int nameExtraPad = 0;
    private string statusBarLabel = "";
    char folderPrefix = '/';

    private bool _isActive;
    private bool _isDiscs;
    private string _path = "";

    private int _offset = 0;
    //private int Offset { get; set; } = 0;
    private int _selected = 0;
    //public int Selected { get; set; } = 0;
    public int Visible { get; set; } = 10;
    #endregion

    #region Properties
    private int Offset
    {
        get { return _offset; }
        set
        {
            if (value < 0) { _offset = 0; return; }
            if (FS_Objects.Count <= Visible){return;}
            if (value >= FS_Objects.Count - Visible) { _offset = FS_Objects.Count - Visible; _selected = FS_Objects.Count - 1; return; }
            _offset = value;
        }
    }

    public int Selected
    {
        get { return _selected; }
        set
        {
            if (value < 0){ return; }
            if (value >= rows.Count - deadRows) { return; }
            _selected = value;
        }
    }
    public bool IsActive
    {
        get { return this._isActive; }
        set { this._isActive = value; }
    }
    public bool IsDiscs
    {
        get { return this._isDiscs; }
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
        _isDiscs = true;
        FS_Objects = FM.SetDiscs();
    }
    private void SetLists(string path)
    {
        _isDiscs = false;
        FS_Objects = FM.SetLists(path);
    }
    
    private void ChangeDir()
    {
        string path = FM.ChangeDir(Path_, GetActiveObject());
        if (path == null) 
            _isDiscs = true;
        else
        {
            Path_ = path;
            _isDiscs = false;
        }
        
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
        halfWindowSize = Console.WindowWidth / 2;
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

    public void Draw()
    {
        ImportRows(); //update long row (laggy)
        List<int> widths = Widths();
        LineLength();
        Visible = Console.WindowHeight - 1 - 1 - 2 - 3 -1; //-1 (Menu) - 3 (Header) - 3 (Status + FKey) - 1 (fKey ofset)
        //var a = new Logs(Visible.ToString());
        halfWindowSize = Console.WindowWidth / 2;

        if (x != 0)
        {
            x = halfWindowSize;
        }

        Console.ForegroundColor = Config.Table_ForegroundColor;
        Console.BackgroundColor = Config.Table_BackgroundColor;

        DrawData(null, widths, l.down, l.lineX, l.topLeft, l.topRight);
        ActivePath();
        DrawData(headers, widths, l.lineY, ' ');
        DrawData(null, widths, l.cross, l.lineX, l.upRight, l.upleft);

        for (int i = Offset; i < Offset + Math.Min(Visible, this.rows.Count); i++)
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

    private void ActivePath()
    {
        ConsoleColor oldTextColor = Console.ForegroundColor;
        ConsoleColor oldBackColor = Console.BackgroundColor;


        Console.SetCursorPosition(x, y_temp - 1);

        string line = @$"{l.topLeft}{l.arrowRight}{l.lineX}"; //@$"┌<─";
        string label = "";

        Console.Write(line);
        if (_isActive)
        {
            Console.ForegroundColor = Config.Table_Path_ACTIVE_ForegroundColor;
            Console.BackgroundColor = Config.Table_Path_ACTIVE_BackgroundColor;
        }
        if (_isDiscs)
            label = " Drives: ";
        else
            label = $" {_path} ";
        Console.Write(label);
        //Console.ResetColor();
        Console.ForegroundColor = oldTextColor;
        Console.BackgroundColor = oldBackColor;
    }

    private void StatusLine(string label) //char vertical = l.lineY //new element?
    {
        string[] local_rows = Generete_StatusLine(label);

        int count = 0;
        foreach (var local_row in local_rows)
        {
            Console.SetCursorPosition(x, y_temp + count);
            Console.Write(local_row);
            count++;
        }
        string diskInfo = "";
        if (Path_.Length >= 3)
            diskInfo = FM.FreeSpace(FM.ActiveDrive(Path_));

        int currentLeftCursor = Console.CursorLeft;
        if (currentLeftCursor > (diskInfo.Length + 2))
            Console.CursorLeft = currentLeftCursor - (diskInfo.Length + 2);
        Console.Write(diskInfo);

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
    public void ImportRows(string path = "")
    {
        deadRows = 0;
        int aaaa = 5;
        if (halfWindowSize >= 29) //need to update
        {
            aaaa = ((halfWindowSize) - 27 - 2);
        }

        if (rows != null)
            rows.Clear();
        foreach (var item in FS_Objects)
        {
            if (item == null)
            {
                LineLength();
                string upDirName = folderPrefix + "..";
                string space = new String(' ', lineLength - upDirName.Length);
                Add(new string[] { upDirName + space, "UP--DIR", "ToDo" });
                continue;
            }
            string name = Truncate.Text(item.Name, aaaa);
            int local_maxNameLength = maxNameLength;

            long size = 0;
            if (item is DirectoryInfo)
            {
                if (!_isDiscs)
                    name = folderPrefix + name;
                local_maxNameLength -= 1;
                
                //try { //missing permision for low level folders
                //    size = FM.DirSize(item as DirectoryInfo);
                //} catch { }
            }
            else
            {
                FileInfo a = item as FileInfo;
                size = a.Length;
            }
            
            Truncate.Text(item.Name, local_maxNameLength); //TODO: clearMe
            
            string sizeStr = "";
            if (size != 0)
                sizeStr = Truncate.Size(size).PadLeft(7);

            Add(new string[] { name, sizeStr, item.LastWriteTime.ToString("MMM dd HH:mm") });
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
    public void Add(string[] data)
    {
        if (data.Length != headers.Count)
            throw new ArgumentException("Invalid columns count");

        rows.Add(new Row(data));
    }
    #endregion


    #endregion
    #region HandleKey methods
    #region Controls
    private void ScrollUp()
    {
        Selected--;

        if (Selected == Offset - 1)
            Offset--;
    }
    private void ScrollDown()
    {
        Selected++;

        if (Selected == Offset + Math.Min(Visible, this.rows.Count))
            Offset++;
    }
    private void GoBegin()
    {
        Selected = 0;
        Offset = 0;
    }
    private void GoEnd()
    {
        Selected = FS_Objects.Count - 1;

        Offset = FS_Objects.Count - Visible;
    }
    private void PageUp()
    {
        //TODO: Opravit podmíky - (out of range)
        //if (offset > Visible)
        //    return;

        Selected = Selected - Visible;
        Offset = Offset - Visible;
    }
    private void PageDown()
    {
        //TODO: Opravit podmíky - (out of range)
        //if (rows.Count < offset + Visible)
        //    return;

        Selected = Selected + Visible;
        Offset = Offset + Visible;
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
        GoBegin();
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
