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
    public char input = 'x';
    protected string _row = "";   //X_totalSize = _x_totalSize;

    //Local
    protected int _x_selected = 0; // current Cursor pos
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
            if (value < 0) { _x_selected = 0; return; }
            //if (value > X_visible) { _x_selected = X_visible; return; }
            //if (value > X_totalSize) { _x_selected = X_totalSize;  return; } //commne out -> broken backspace Cursor
            _x_selected = value;
        }
    }
    public int X_offset
    {
        get { return _x_offset; }
        set
        {
            if (value < 0) { _x_offset = 0; return; }
            if (X_totalSize <= X_visible) { _x_offset = 0; return; }
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

    public void Right(string nextRow)
    {
        if (Row.Length - 1 == X_selected) //up?
        {
            Down(nextRow);
            GoBegin();
            return;
        }

        X_selected++;

        if (X_selected == X_offset + Math.Min(X_visible, X_totalSize))
            X_offset++;
    }
    public void Left(string nextRow)
    {
        
        if (0 == X_selected)
        {
            bool RowIsSame = false;
            if (Row == nextRow)
                RowIsSame = true;

            Up(nextRow);

            if (RowIsSame)
                GoBegin();
            else
                GoEnd(nextRow.Length);
            return;
        }

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
    public void GoEnd(int localX_totalSize)
    {
        X_selected = localX_totalSize - 1;
        X_offset = localX_totalSize - X_visible;
    }
    public void GoToLine(int lineIndex_Y, int x = 0)
    {
        Y_offset = lineIndex_Y - 1;
        Y_selected = lineIndex_Y - Y_offset;

        //add X_offset
        X_selected = x;
    }
    #endregion

    protected void CheckLineSize(string nextRow)
    {
        Row = nextRow;
        X_offset = 0;
        if (X_totalSize < X_selected)
        {
            _x_selected = X_totalSize - 1;
        }
    }

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
        if (Row.Length == 0)
        {
            input = ' ';
            X_selected = 0;
            X_offset = 0;
        }
        else
        {
            input = GetActiveChar();
        }
        try
        {
            Console.SetCursorPosition((X_selected - X_offset) + X_start, (Y_selected - Y_offset) + Y_start);
        }
        catch (Exception)
        {
            
        }
        
        
        Console.Write(input);


        #region Restore
        Console.CursorLeft = Old_X;
        Console.CursorTop = Old_Y;
        Console.BackgroundColor = Old_BG;
        Console.ForegroundColor = Old_FG;
        #endregion
    }
    
    public char GetActiveChar()
    {
        if (X_selected  > Row.Length - 1) // ? X_selected + X_offset => X_selected //why?
        {
            X_selected = Row.Length - 1;
        }
        return Row[X_selected];
    }
}
