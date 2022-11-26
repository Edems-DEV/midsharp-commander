using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Cursor_2D : Cursor_1D //(1D + X ['-'] )
{
    public Cursor_2D(int y_start, int maxHeight, int rowsCount, int _x_start, int _x_visible) : base(y_start, maxHeight, rowsCount)
    {
        X_start = _x_start;
        X_visible = _x_visible;
        //X_totalSize = _x_totalSize;
    }

    #region Atributes
    //Feed me
    protected int _x_start;        // original pos (for reset)
    protected int _x_visible;      // maxHeight
    protected int _x_totalSize;    // rows.count
    protected char input = 'x';
    protected string _row = "";   //X_totalSize = _x_totalSize;

    //Local
    protected int _x_selected = 0; // current cursor pos
    protected int _x_offset = 0;   // offset
    #endregion
    #region Properties 
    public string Row
    {
        get { return _row; }
        set
        {
            _row = value;
        }
    }
    public int X_selected
    {
        get { return _x_selected; }
        set
        {
            if (value < 0) { return; }
            if (value >= Row.Length) { return; }
            if (value > X_totalSize) { _x_selected = X_totalSize;  return; }
            _x_selected = value;
        }
    }
    public int X_offset
    {
        get { return _x_offset; }
        set
        {
            if (value < 0) { _x_offset = 0; return; }
            if (X_totalSize <= X_visible) { return; }
            if (value >= X_totalSize - X_visible) { _x_offset = X_totalSize - X_visible; X_selected = X_totalSize - 1; return; }

            _x_offset = value;
        }
    }
    public int X_visible
    {
        get { return _x_visible; }
        set
        {
            _x_visible = value;
        }
    }
    public int X_start
    {
        get { return _x_start; }
        set
        {
            _x_start = value;
        }
    }
    public int X_totalSize
    {
        get { return Row.Length; }
        //set
        //{
        //    _x_totalSize = value;
        //}
    }
    #endregion
    #region Methods
    protected void CheckLineSize(string nextRow)
    {
        Row = nextRow;
        if (X_totalSize < X_selected)
        {
            _x_selected = X_totalSize - 1;
        }
    }
    public override void Up()
    {
        
        base.Up();
    }
    public override void Down()
    {
        base.Down();
        //go to end of line (no empthy space)
    }
    public void Up(string nextRow)
    {
        CheckLineSize(nextRow);
        base.Up();
    }
    public void Down(string nextRow)
    {
        CheckLineSize(nextRow);
        base.Down();
    }

    public void Right()
    {
        X_selected++;

        if (X_selected == X_offset + Math.Min(X_visible, X_totalSize))
            X_offset++;
    }
    public void Left()
    {
        X_selected--;

        if (X_selected == X_offset - 1)
            X_offset--;
    }

    public override void GoBegin()
    {
        X_selected = 0;
        X_offset = 0;
    }
    public override void GoEnd()
    {
        X_selected = X_totalSize - 1;
        X_offset = X_totalSize - X_visible;
    }
    #endregion
    public void Draw()
    {
        if (input == null)
            return;

        #region SaveOldSettings
        int Old_X = Console.CursorLeft;
        int Old_Y = Console.CursorTop;
        ConsoleColor Old_BG= Console.BackgroundColor;
        ConsoleColor Old_FG = Console.ForegroundColor;
        #endregion


        Console.BackgroundColor = Config.Table_Line_ACTIVE_BackgroundColor;
        Console.ForegroundColor = Config.Table_Line_ACTIVE_ForegroundColor;
        Console.SetCursorPosition((X_selected - X_offset) + X_start, (Y_selected - Y_offset) + Y_start);
        Console.Write(input);


        #region Restore
        Console.CursorLeft = Old_X;
        Console.CursorTop = Old_Y;
        Console.BackgroundColor = Old_BG;
        Console.ForegroundColor = Old_FG;
        #endregion
    }
}
