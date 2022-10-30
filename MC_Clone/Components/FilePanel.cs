using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    MenuBar MenuBar = new MenuBar();

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
    private string _path = Config.FOLDER;

    string statusBarLabel = "";

    #region Static
    char folderPrefix = '/';
    //char verticalLine = '│'; //all frame chars here
    int haldWindow = 0;
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

    void Start(int X = 0, int Y = 0)
    {
        x = X;
        y = Y;
        y_temp = y;
        LineLength();
        haldWindow = Console.WindowWidth / 2;

    }
    public FilePanel(int X = 0, int Y = 0)
    {
        Start(X, Y);
        SetDiscs();
    }

    public FilePanel(string path, int X = 0, int Y = 0)
    {
        Start(X, Y);
        SetLists();
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
        if (MenuBar.active)
        {
            MenuBar.HandleKey(info);
            return;
        }
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
                if (FS_Objects[Selected] is DirectoryInfo) { goto case ConsoleKey.Enter; break; }
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
                MenuBar.active = true;
                PullDn();
                break;
            case ConsoleKey.F10:
                goto case ConsoleKey.Escape;
            case ConsoleKey.Escape:
                this.Quit();
                break;
        }
    }
    public void StatusLine(string label) //char vertical = '│'
    {
        string row0 = ""; //need to overrite coners
        row0 += '├';
        row0 += new String('─', lineLength - 2);
        row0 += '┤';
        string row1 = "";
        row1 += '│';
        if (label == folderPrefix + "..")
            label = "UP--DIR";
        else if (!label.StartsWith(folderPrefix))
            label = " " + label; 
        row1 += label; //modify for each file type
        row1 += new String(' ', lineLength - row1.Length - 1);
        row1 += '│';
        string row2 = "";
        row2 += '└';
        row2 += new String('─', lineLength - 2);
        row2 += '┘';

        string[] local_rows = { row0, row1, row2 };

        int count = 0;
        foreach (var local_row in local_rows)
        {
            Console.SetCursorPosition(x, y_temp + count);
            Console.Write(local_row);
            count++;
        }
        string diskInfo = freeSpace();
        int currentLeftCursor = Console.CursorLeft;
        if (currentLeftCursor > (diskInfo.Length + 2))
            Console.CursorLeft = currentLeftCursor - (diskInfo.Length + 2);
        Console.Write(diskInfo);

    }

    string freeSpace() // 52G/58G (89%)
    {
        //TODO:
        // - use SizeConvertor() (+ this will show other sizes, no only GB )
        // - use SetDiscs(); -> dynamic disk select (at the moment it shows only disk[0] -> "C:\" )
        //SetDiscs();
        //SizeConvertor();
        string name = "";
        const double GB = 1073741824; //bytesToGb

        DriveInfo[] allDrives = DriveInfo.GetDrives();
        var ActiveDrive = allDrives[0];

        double freeSpacePerc = Math.Round((ActiveDrive.AvailableFreeSpace / (float)ActiveDrive.TotalSize) * 100, 0);
        int Total = Convert.ToInt32(ActiveDrive.TotalSize / GB);
        int Free = Convert.ToInt32(ActiveDrive.TotalFreeSpace / GB);

        name = $" {Free}G/{Total}G ({freeSpacePerc}%) ";

        return name;
    }
    void activePath()
    {
        Console.SetCursorPosition(x, y_temp - 1);

        string line = @$"┌<─";
        string label = "";

        Console.Write(line);
        if (_active) {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
        }
        if (_discs)
            label = " Drives: ";
        else
            label = $" {_path} ";
        Console.Write(label);
        Console.ResetColor();
    }

    public void Draw()
    {
        ImportRows(); //update long row (laggy)
        List<int> widths = Widths();
        LineLength();
        Visible = Console.WindowHeight - 1 - 1 - 2 - 3 -1; //-1 (Menu) - 3 (Header) - 3 (Status + FKey) - 1 (fKey ofset)
        //var a = new Logs(Visible.ToString());
        haldWindow = Console.WindowWidth / 2;

        if (x != 0)
        {
            x = haldWindow;
        }


        MenuBar.Draw();

        DrawData(null, widths, '┬', '─', '┌', '┐');
        activePath();
        DrawData(headers, widths, '│', ' ');
        DrawData(null, widths, '┼', '─', '├', '┤');

        for (int i = offset; i < offset + Math.Min(Visible, this.rows.Count); i++)
        {
            if (i == Selected)
            {
                Console.ForegroundColor = Config.FocusText;
                Console.BackgroundColor = Config.FocusBackgroud;
                statusBarLabel = rows[i].Data[0];
            }
            
            DrawData(rows[i].Data, widths, '│', ' ');

            Console.ResetColor();
        }

        //DrawData(null, widths, '┴', '─', '└', '┘');
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

    public int LineLength()
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
        Console.WriteLine("HalfWIn= " + HalfWIn + "| lineLength=" + lineLength + "| maxNameLength=" + maxNameLength + "| pad= "  + (HalfWIn - lineLength));
        //var a = new Logs(lenght.ToString());
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

    public void ImportRows(string path = "")
    {
        deadRows = 0;
        int aaaa = 5;
        if (haldWindow >= 29) //need to update
        {
            aaaa = ((haldWindow) - 27 - 2);
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
            string name = Truncated(item.Name, aaaa);
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
            
            Truncated(item.Name, local_maxNameLength);
            Add(new string[] { name, SizeConvertor(size), item.LastWriteTime.ToString("MMM dd HH:mm") });
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
    string Truncated(string ts, int maxLength, string trun = "~")
    {
        if (ts.Length < maxLength)
        {
            //if (Console.BufferWidth > lineLength)
            //{
            //    int pad = (Console.BufferWidth - lineLength);
            //    string space = new String(' ', pad);
            //    return ts + space;
            //}
            return ts;
        }
            
        maxLength = maxLength - trun.Length;
        int a = maxLength / 2 + maxLength % 2;
        int b = maxLength / 2;
        var truncated = ts.Substring(0, a) + trun + ts.Substring(ts.Length - b, b);

        return truncated;
    }
    
    string SizeConvertor(long Bytes) //max line zize  = 7 (123,45K)
    {
        double bytes = Bytes;
        int r = 2;
        if (bytes < 1000)
            return bytes + "B";
        if (bytes > 1000 && bytes < 1000000)
            return Math.Round(bytes / 1000, r) + "K";
        if (bytes > 1000000 && bytes < 1000000000)
            return Math.Round(bytes / 1000000, r) + "M";
        if (bytes > 1000000000 && bytes < 1000000000000)
            return Math.Round(bytes / 1000000000, r) + "G";
        if (bytes > 1000000000000)
            return Math.Round(bytes / 1000000000000,r) + "T";
        return " ";
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
        this.Draw();
    }
    private void CreateFile() //Menu
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
            SetLists();
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
            SetLists();
            ImportRows();
            this.Draw();
        }
        catch (Exception e)
        {
            this.ShowMessage(e.Message);
            return;
        }
    }
    private void CopyDirectory(string sourceDirName, string destDirName)
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
            SetLists();
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
            SetLists();
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
        PrintString(message, 0, Console.WindowHeight -2, ConsoleColor.Green, ConsoleColor.Black);
    }

    public static void PrintString(string str, int X, int Y, ConsoleColor text, ConsoleColor background)
    {
        Console.ForegroundColor = text;
        Console.BackgroundColor = background;

        Console.SetCursorPosition(X, Y);
        Console.Write(str);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.BackgroundColor = ConsoleColor.Black;
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
            SetLists();
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
    
    
    

    public void SetLists()
    {
        if (this.FS_Objects.Count != 0)
        {
            this.FS_Objects.Clear();
        }
        this._discs = false;
        DirectoryInfo levelUpDirectory = null;
        this.FS_Objects.Add(levelUpDirectory);
        //Directories
        string[] directories = Directory.GetDirectories(this._path);
        foreach (string directory in directories)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            this.FS_Objects.Add(di);
        }
        //Files
        string[] files = Directory.GetFiles(this._path);
        foreach (string file in files)
        {
            FileInfo fi = new FileInfo(file);
            this.FS_Objects.Add(fi);
        }
    }
    public void SetDiscs()
    {
        if (this.FS_Objects.Count != 0)
        {
            this.FS_Objects.Clear();
        }
        this._discs = true;
        DriveInfo[] discs = DriveInfo.GetDrives();
        foreach (DriveInfo disc in discs)
        {
            if (disc.IsReady)
            {
                DirectoryInfo di = new DirectoryInfo(disc.Name);
                this.FS_Objects.Add(di);
            }
        }
    }

    public FileSystemInfo GetActiveObject()
    {
        if (this.FS_Objects != null && this.FS_Objects.Count != 0)
        {
            return this.FS_Objects[this.Selected];
        }
        throw new Exception("The list of panel objects is empty");
    }

    private void ChangeDir()
    {
        FileSystemInfo fsInfo = this.FS_Objects[this.Selected]; //GetActiveObject();
        if (fsInfo != null)
        {
            if (fsInfo is DirectoryInfo)
            {
                //try{ Directory.GetDirectories(fsInfo.FullName); } //uselles?
                //catch{ return; }

                Path_ = fsInfo.FullName;
                SetLists();
                UpdatePanel();
            }
            //else
            //    //file -> F4 edit
        }
        else
        {
            string currentPath = Path_;
            DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);
            DirectoryInfo upLevelDirectory = currentDirectory.Parent;

            if (upLevelDirectory != null)
            {
                Path_ = upLevelDirectory.FullName;
                SetLists();
                UpdatePanel();
            }

            else
            {
                SetDiscs();
                UpdatePanel();
            }
        }
    }
    public void UpdatePanel()
    {
        offset = 0;
        Selected = 0;
        Clear(); //change to something better (clear only that pane)
        ImportRows();
        this.Draw();
    }

    void Clear()
    {
        int rows = Visible + headers.Count + 1 + 2; //final line + 2 Menu

        string space = new String(' ', lineLength);
        for (int i = 0; i < rows; i++)
        {
            Console.SetCursorPosition(x, y + i);
            Console.WriteLine(space);
        }
    }
}
