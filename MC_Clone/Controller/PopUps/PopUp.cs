namespace MC_Clone;

public abstract class PopUp_Red : PopUp //TODO: find better name
{
    public PopUp_Red()
    {
        BackgroundColor = Config.Error_Backgroud;
        ForegroundColor = Config.Error_Foreground;
        AccentColor = Config.Error_Accent;
        Active = Config.Error_ACTIVE;
    }
}

public abstract class PopUp : Window //?: Window -> have same needs
{
    private int selected = 0;

    #region Atributes
    public int originalY;
    public int originalX;

    public int x_center = 0; // -
    public int y_center = 0; // |

    public int height = 2 + 8 + 8;
    public int width = 0;
    private int maxWidth = Console.WindowWidth / 3;

    public int X_DeadSpace = 5;

    public string title = "";
    public List<string> details = new List<string>();
    protected List<IComponent> components = new List<IComponent>();
    //public List<Button> buttons = new List<Button>();
    //public List<TextBox> textBoxes = new List<TextBox>();

    public ConsoleColor BackgroundColor = Config.PopUp_Backgroud;
    public ConsoleColor ForegroundColor = Config.PopUp_ForeGroud;
    public ConsoleColor AccentColor = Config.PopUp_Accent;
    public ConsoleColor Active = Config.PopUp_ACTIVE;

    Application app;
    #endregion
    #region Property
    public int X
    {
        get { return x_center - width / 2; }
        set { x_center = value + width / 2; }
    }

    public string Info
    {
        get { return string.Join("\n", details); }
        set
        {
            if (value.Contains("\n"))
            {
                details = value.Split("\n").ToList();
                //infos.AddRange(value.Split("\n"));
            }
            else
            {
                details.Add(value);
            }
        }
    }

    public int Selected
    {
        get { return selected; }
        set
        {
            if (value < 0)
                selected = 0;
            else if (value >= components.Count)
                selected = components.Count - 1;
            else
                selected = value;
        }
    }
    #endregion
    #region Constructor
    private void Setup(string title, int width = 50)
    {
        x_center = Console.WindowWidth / 2;
        y_center = Console.WindowHeight / 3;

        this.width = width;
        this.title = title;
    }
    public PopUp()
    {
        x_center = Console.WindowWidth / 2;
        y_center = Console.WindowHeight / 3;
    }
    public PopUp(string title, string info = "", int width = 50)
    {
        Setup(title, width);
        Info = info;
    }
    public PopUp(string title, List<string> info, int width = 50)
    {
        Setup(title, width);
        details = info;
    }
    #endregion

    public override void HandleKey(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            case ConsoleKey.Tab:
                Selected = (Selected + 1) % components.Count;
                return;
            case ConsoleKey.RightArrow:
            case ConsoleKey.DownArrow:
                Selected += 1;
                return;
            case ConsoleKey.LeftArrow:
            case ConsoleKey.UpArrow:
                Selected -= 1;
                return;
            case ConsoleKey.Escape:
                BtnCancel_Clicked();
                return;
        }

        components[Selected].HandleKey(info);
    }
    public void Add_CancelBtn()
    {
        Button btnCancel = new Button() { Title = "Cancel" };
        btnCancel.Clicked += BtnCancel_Clicked;
        components.Add(btnCancel);
    }
    public void BtnCancel_Clicked()
    {
        this.Application.SwitchPopUp(new EmptyMsg());
    }
    public void Add_BtnOk()
    {
        Button btnOk = new Button() { Title = "OK" };
        btnOk.Clicked += BtnOk_Clicked;
        components.Add(btnOk);
    }
    
    protected virtual void BtnOk_Clicked()
    {
        
    }

    #region Error specific
    public void Add_CancelBtn(ConsoleColor AsscentColor)
    {
        Button btnCancel = new Button() { Title = "Cancel" };
        btnCancel.Clicked += BtnCancel_Clicked;
        btnCancel.asccentColor = AsscentColor;
        components.Add(btnCancel);
    }
    public void Add_BtnOk(ConsoleColor AsscentColor)
    {
        Button btnOk = new Button() { Title = "OK" };
        btnOk.Clicked += BtnOk_Clicked;
        btnOk.asccentColor = AsscentColor;
        components.Add(btnOk);
    }
    #endregion
    
    public override void Draw()
    {
        List<TextBox> textBoxes = components.OfType<TextBox>().ToList();

        bool a = true;
        if (a)
        {
            x_center = Console.WindowWidth / 2; //each draw it update by window size
            y_center = Console.WindowHeight / 3;
            
            height = WidthCalc(); //cant be in constructor because elemets are added in run time
            width = MinWidth();
            //a = false;

            if (width <= 6) //6 = empty msg width
            {
                return;
            }
        }
        Console.BackgroundColor = BackgroundColor;
        int oldY = y_center;

        DrawBox(); //+2 (space + box)
        DrawTitle(); //own -1
        DrawInfo(); //+1
        if (textBoxes.Count > 0)
        {
            DrawTextBoxes();
        }

        y_center += Line('├', '┤', '─');
        DrawButtonsRow();

        y_center = oldY;

        //Console.SetCursorPosition(0, 0); //blinking cursor
    }

    public void Draw(string title, string info, int width = 50)
    {
        this.title = title;
        Info = info;
        Draw();
    }

    public void DrawBox()
    {
        int yy = 0;
        int ySize;

        yy += Line(' ', ' ', ' ', yy);
        yy += Line('┌', '┐', '─', yy);
        ySize = yy;

        while (yy < height - ySize)
        {
            yy += Line('│', '│', ' ', yy);
        }
        yy += Line('└', '┘', '─', yy);
        yy += Line(' ', ' ', ' ', yy);

        y_center += ySize;
    }

    public void DrawTitle()
    {
        ConsoleColor oldTextC = Console.ForegroundColor; //del this - always same (cfg)
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.ForegroundColor = AccentColor;
        Console.BackgroundColor = BackgroundColor;

        string local_title = $" {title} "; //spaces //fixed bug - on interation -> spaces added

        try
        {
            Console.SetCursorPosition(XCenter(local_title), y_center - 1);
        }
        catch { throw new Exception($"{XCenter(local_title)}"); }
        Console.Write(local_title);

        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;
    }
    public void DrawInfo()
    {
        ConsoleColor oldTextC = Console.ForegroundColor;
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.ForegroundColor = ForegroundColor;
        Console.BackgroundColor = BackgroundColor;

        foreach (var line in details)
        {
            Console.SetCursorPosition(XCenter(line), y_center);
            Console.Write(line);
            y_center++;
        }


        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;
    }

    public int Line(char start = '└', char end = '┘', char fill = '─', int yy = 0)
    {
        ConsoleColor oldBackC = Console.BackgroundColor;
        ConsoleColor oldTextC = Console.ForegroundColor;
        Console.BackgroundColor = BackgroundColor;
        Console.ForegroundColor = ForegroundColor;


        string buf = "";

        buf += " ";
        buf += start;
        for (int i = 0; i <= width - X_DeadSpace; i++) //-1 -> '<='
        {
            buf += fill;
        }
        buf += end;
        buf += " ";
        Console.SetCursorPosition(XCenter(buf), y_center + yy);
        Console.Write(buf);


        Console.BackgroundColor = oldBackC;
        Console.ForegroundColor = oldTextC;

        int ySize = 1;
        return ySize;
    }

    public void DrawTextBoxes()
    {
        List<TextBox> textBoxes = components.OfType<TextBox>().ToList();

        ConsoleColor oldTextC = Console.ForegroundColor;
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.ForegroundColor = ForegroundColor;
        Console.BackgroundColor = BackgroundColor;

        int ySize = 0;

        int i = 0;
        foreach (var textbox in textBoxes)
        {
            y_center += Line('├', '┤', '─');
            textbox.Size = width - X_DeadSpace;
            if (Selected == i)
            {
                Console.BackgroundColor = Active;
            }
            y_center += textbox.Draw(XCenter(width) + 2, y_center);
            Console.BackgroundColor = BackgroundColor;
            i++;
        }
    }

    public void DrawButtonsRow()
    {
        List<Button> buttons = components.OfType<Button>().ToList();

        ConsoleColor oldTextC = Console.ForegroundColor;
        Console.ForegroundColor = ForegroundColor;
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.BackgroundColor = BackgroundColor;

        int TotalWidth = 0;
        int space = 2;
        foreach (var button in buttons)
        {
            TotalWidth += button.Title.Length + 4;
            TotalWidth += space;
        }
        TotalWidth -= 5; //idk - just works to center it

        Console.SetCursorPosition(XCenter(TotalWidth), y_center);
        int i = components.Count - buttons.Count; //works only if buttos are last elements added
        foreach (var button in buttons)
        {
            if (Selected == i)
            {
                Console.BackgroundColor = Active;
            }
            button.Draw();
            Console.BackgroundColor = BackgroundColor;
            Console.Write(" ");
            i++;
        }

        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;

        y_center++; //single row
    }

    public void Clear()
    {
        Console.Clear();
        x_center = Console.WindowWidth / 2;
        y_center = Console.WindowHeight / 3; //reset y
    }

    public int XCenter(string text)
    {
        return x_center - text.Length / 2;
    }
    public int XCenter(int text)
    {
        return x_center - text / 2;
    }

    public void Write(string text)
    {
        Console.SetCursorPosition(XCenter(text), y_center);
        Console.Write(text);
        y_center++;
    }

    public int WidthCalc()
    {
        List<TextBox> textBoxes = components.OfType<TextBox>().ToList();
        List<Button> buttons = components.OfType<Button>().ToList();

        int local_width = 0;
        local_width += 2;//start box + space
        foreach (var item in details)
        {
            local_width += 1;
        }
        foreach (var item in textBoxes)
        {
            local_width += 1; //seperator
            local_width += 2;
        }
        if (buttons.Count > 0)
        {
            local_width += 1; //seperator
            local_width += 1;
        }
        local_width += 2; //end box + space


        return local_width;
    }
    public int MinWidth(int ExtraPad = 2)
    {
        List<TextBox> textBoxes = components.OfType<TextBox>().ToList();
        List<Button> buttons = components.OfType<Button>().ToList();
        //for each elemnt size x size
        int maxWidth = 0;
        foreach (var item in details)
        {
            if (item.Length > maxWidth)
            {
                maxWidth = item.Length;
            }
        }
        foreach (var item in textBoxes)
        {
            if (item.Label.Length > maxWidth)
            {
                maxWidth = item.Label.Length;
            }
            //if (item.Value.Length > maxWidth)
            //{
            //    maxWidth = item.DisplayValue.Length; //user input pontetional problem?
            //}
        }
        if (buttons.Count > 0)
        {
            int localBtnRowLenght = 0;
            foreach (var item in buttons) //row
            {
                localBtnRowLenght += item.Title.Length + 4 + 2;
            }
            if (localBtnRowLenght > maxWidth)
            {
                maxWidth = localBtnRowLenght;
            }
        }
        if (title.Length > maxWidth)
        {
            maxWidth = title.Length;
        }
        if (maxWidth < 0)
        {
            throw new Exception($"maxWidth < 0  [{maxWidth}]");
        }
        maxWidth += 4; //box + space
        maxWidth += 2; //%2 = 0 (property?)
        return maxWidth;
    }
}

