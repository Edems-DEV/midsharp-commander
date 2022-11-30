namespace MC_Clone;

public class FilePanel : IComponent
{
    public ListWindow listWindow { get; set; }
    public Application Application { get; set; }

    public event Action<int> RowSelected;


    private FileManager FM;
    private List<int> widths;
    private List<Row> rows = new List<Row>();
    private List<FileSystemInfo> FS_Objects = new List<FileSystemInfo>();
    // Size and Date are always trunced to 7 & 12 (TODO: del width -> use this)
    private List<string> headers = new List<string>(new string[] { "Name", Misc.PadBoth("Size", 7), Misc.PadBoth("Date", 12) });

    #region Atributes
    private int deadRows = 0;
    private int lineLength = 0; //used in: Clear(), Generete_StatusLine()
    private int maxNameLength = 0; // used in: Truncate.name
    private string statusBarLabel = "";
    char folderPrefix = '/';


    private int _x = 0;
    private int _y = 0;
    private int y_temp = 0;

    private bool _isActive;
    private bool _isDiscs;
    private string _path = "";

    private int _offset = 0;
    private int _selected = 0;
    private int _visible = 10;
    #endregion

    #region Properties
    private int halfWindowSize
    {
        get { return Console.BufferWidth / 2; } //always calcul -> performance? (fixed buggy size?)
    }

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
            if (FS_Objects.Count <= Visible) { return; }
            if (value >= FS_Objects.Count - Visible) { _offset = FS_Objects.Count - Visible; _selected = FS_Objects.Count - 1; return; }
            _offset = value;
        }
    }
    public int Selected
    {
        get { return _selected; }
        set
        {
            if (value < 0) { return; }
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

    #region Constructor
    void Start(int x = 0, int y = 0)
    {
        X = x;
        Y = y;
        y_temp = y;
        UpdateMaxLengths();
        FM = new FileManager();
        //set lisener for if window size changed -> update rows and their length +...
        OnResize();
        Application.WinSize.OnWindowSizeChange += OnResize;
    }
    public FilePanel(Application Application, int x = 0, int y = 0)
    {
        this.Application = Application;
        Start(x, y);
        SetDiscs();
    }

    public FilePanel(Application Application, string path, int x = 0, int y = 0)
    {
        this.Application = Application;
        Path_ = path;
        Start(x, y);
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
            //---------FUNCTION---------
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
            case ConsoleKey.Delete:
            case ConsoleKey.F8:
                Delete();
                break;
            case ConsoleKey.F9:
                //MenuBar.active = true;
                //PullDn();
                break;
            case ConsoleKey.F10:
            case ConsoleKey.Escape:
                Quit();
                break;
        }
    }

    public void Draw()
    {
        if (!IsDiscs) //temp fix (need: event if data were changed  <- popUps)
        {
            SetLists(_path); //TODO: change me (draw no -> logic)
            ImportRows();
        }

        if (X != 0)
        {
            X = halfWindowSize;
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
        y_temp = Y;
    }
    #region Draw methods
    private void DrawData(List<string>? data, List<int> widths, char sep, char pad, char start = 'ĉ', char end = 'ĉ')
    {
        char _start = start == 'ĉ' ? sep : start;
        char _end = end == 'ĉ' ? sep : end;

        int i = 0;

        Console.SetCursorPosition(X, y_temp);
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


        Console.SetCursorPosition(X, y_temp - 1);

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

        Console.ForegroundColor = oldTextColor;
        Console.BackgroundColor = oldBackColor;
    }

    private void StatusLine(string label) //new element?
    {
        string[] local_rows = Generate_StatusLine(label);

        int count = 0;
        foreach (var local_row in local_rows)
        {
            Console.SetCursorPosition(X, y_temp + count);
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
    private string[] Generate_StatusLine(string label)
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

    public void OnResize()
    {
        ImportRows(); //update rows
        widths = Widths();
        UpdateMaxLengths();
        Visible = Console.WindowHeight - 1 - 1 - 2 - 3 - 1; //-1 (Menu) - 3 (Header) - 3 (Status + FKey) - 1 (fKey ofset)
    }
    public void UpdateMaxLengths() // why? So I don't have to calculate that for each line - PERFORMANCE?
    {
        GetMaxLineLength();
        GetMaxNameLength();
    }

    private int GetMaxLineLength(int startIndex = 0)
    {
        lineLength = halfWindowSize;
        return halfWindowSize;
    }
    private int GetMaxNameLength()
    {
        int local_maxNameLength = halfWindowSize;
        local_maxNameLength -= 1; // first |
        local_maxNameLength -= 26; // | Size | Date |

        maxNameLength = local_maxNameLength;
        return local_maxNameLength;
    }
    private int new_MaxNameLength()
    {
        int local_maxNameLength;
        local_maxNameLength = halfWindowSize;
        local_maxNameLength -= 1; // first |
        for (int i = 1; i < headers.Count; i++)
        {
            local_maxNameLength -= headers[i].Length;
            local_maxNameLength -= 1; // '|'
        }
        maxNameLength = local_maxNameLength;
        return local_maxNameLength;
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
        UpdateMaxLengths();
        LongLine();

        if (rows != null)
            rows.Clear();

        AddFormatedRows();

        AddDeadRows();
    }
    private void AddFormatedRows()
    {
        foreach (var item in FS_Objects)
        {
            if (item == null)
            {
                string upDirName = folderPrefix + "..";
                upDirName = Truncate.Text(upDirName, maxNameLength);
                Add(new string[] { upDirName, "UP--DIR", "ToDo" });
                continue;
            }
            string name = Truncate.Text(item.Name, maxNameLength);
            int local_maxNameLength = maxNameLength;

            long size = 0;
            if (item is DirectoryInfo)
            {
                if (!_isDiscs)
                    name = folderPrefix + name;
                local_maxNameLength -= 1;
                Truncate.Text(item.Name, local_maxNameLength);

                //try { //missing permision for low level folders
                //    size = FM.DirSize(item as DirectoryInfo);
                //} catch { }
            }
            else
            {
                FileInfo a = item as FileInfo;
                size = a.Length;
            }

            string sizeStr = "";
            if (size != 0)
                sizeStr = Truncate.Size(size).PadLeft(7);

            Add(new string[] { name, sizeStr, item.LastWriteTime.ToString("MMM dd HH:mm") });
        }
    }

    private void AddDeadRows()
    {
        deadRows = 0;

        int yElementsSize = 8; // Menu, Header, StatusLine, ... //TODO: global atribut
        int occupiedSpace = rows.Count + yElementsSize;
        for (int i = 0; i < Console.WindowHeight - occupiedSpace; i++)
        {
            Add(new string[] { "", "", "" });
            deadRows++;
        }
    }

    public void Add(string[] data)
    {
        if (data.Length != headers.Count)
            throw new ArgumentException("Invalid columns count");

        rows.Add(new Row(data));
    }

    private void LongLine()
    {
        headers[0] = Truncate.Text(FillToLong("name"), GetMaxNameLength());
    }

    private string FillToLong(string text)
    {
        UpdateMaxLengths();

        string emptyLong = "";
        int maxE = GetMaxNameLength() - text.Length + 2; //+2 fixed GetMaxNameLength() too short (idk why)
        if (0 < maxE)
        {
            emptyLong = new String(' ', maxE);
        }

        return Misc.PadBoth(text, maxE);
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
        Selected = Selected - Visible;
        Offset = Offset - Visible;
    }
    private void PageDown()
    {
        Selected = Selected + Visible;
        Offset = Offset + Visible;
    }
    #endregion
    #region FunctionKeys
    private void Drives() //Help
    {
        //SetDiscs();
        //RefreshPanel();
        Application.SwitchPopUp(new ErrorMsg("I can acces App from FilePanel"));
    }
    private void CreateFile() //Menu //IDEA: move closer to MkDir?
    {
        if (IsDiscs)
            return;
        Application.SwitchPopUp(new CreateFileMsg(Path_));
    }
    private void View()
    {
        if (IsDiscs)
            return;
        if (GetActiveObject() is not FileInfo)
            return;

        FileInfo file = GetActiveObject() as FileInfo;

        Application.WinSize.OnWindowSizeChange -= OnResize;
        Application.SwitchWindow(new PreviewWindow(Application, file));
    }
    private void Edit()
    {
        if (IsDiscs)
            return;
        if (GetActiveObject() is not FileInfo)
            return;

        FileInfo file = GetActiveObject() as FileInfo;

        Application.WinSize.OnWindowSizeChange -= OnResize;
        Application.SwitchWindow(new EditWindow(Application, file));
    }
    private void Copy()
    {
        Application.SwitchPopUp(new CopyMsg(GetActiveObject()));
    }
    private void RenMov()
    {
        Application.SwitchPopUp(new MoveMsg(GetActiveObject()));
    }
    private void MkDir()
    {
        if (IsDiscs)
            return;
        Application.SwitchPopUp(new CreateFolderMsg(Path_));
    }
    private void Delete()
    {
        if (IsDiscs)
            return;
        Application.SwitchPopUp(new DeleteMsg(GetActiveObject()));
        Selected -= 1; //move before deleted (backgroud still updates) //TODO: Fix
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
    #region Misc
    public FileSystemInfo GetActiveObject()
    {
        if (FS_Objects != null && FS_Objects.Count != 0)
        {
            return FS_Objects[Selected];
        }
        throw new Exception("The list of panel objects is empty");
    }

    private void RestartPanel()
    {
        SetLists(Path_);
        UpdatePanel();
    }
    private void RefreshPanel()
    {
        GoBegin();
        UpdatePanel();
    }
    private void UpdatePanel()
    {
        Clear();
        ImportRows();
        Draw();
    }

    private void Clear()
    {
        int rows = Visible + headers.Count + 1 + 2; //final line + 2 Menu

        string space = new String(' ', lineLength);
        for (int i = 0; i < rows; i++)
        {
            Console.SetCursorPosition(X, Y + i);
            Console.WriteLine(space);
        }
    }
    #endregion
}