using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Cursor_1D //: IComponent //(Y '|')
{
    #region Atributes
    //RENAME
    //Selected;          // Y_selected
    //offset;            // Y_offset
    //Y_visible;         // Y_visible
    //Y_totalSize;        // Y_totalSize
    //FS_Objects.count;  // Y_totalSize (-1)

    public Cursor_1D(int y_start, int maxHeight, int rowsCount)
    {
        Y_start = y_start;
        Y_visible = maxHeight;
        Y_totalSize = rowsCount;
    }

    //Feed me
    protected int _y_start;        // original pos (for reset)
    protected int _y_visible;      // maxHeight
    protected int _y_totalSize;    // Y_totalSize

    //Local
    protected int _y_selected = 0; // current Cursor pos
    protected int _y_offset = 0;   // offset
    #endregion
    #region Properties 
    public int Y_selected
    {
        get { return _y_selected; }
        set
        {
            if (value < 0) { return; }
            if (value >= Y_totalSize) { return; }
            _y_selected = value;
        }
    }
    public int Y_offset
    {
        get { return _y_offset; }
        set
        {
            if (value < 0) { _y_offset = 0; return; }
            if (Y_totalSize <= Y_visible) { return; }
            if (value >= Y_totalSize - Y_visible) { _y_offset = Y_totalSize - Y_visible; Y_selected = Y_totalSize - 1; return; }

            _y_offset = value;
        }
    }
    public int Y_visible
    {
        get { return _y_visible; }
        set
        {
            if (value < 0)
                throw new Exception(String.Format($"Visible < 0 (val: {value})"));
            _y_visible = value;
        }
    }
    public int Y_start
    {
        get { return _y_start; }
        set
        {
            _y_start = value;
        }
    }
    public int Y_totalSize
    {
        get { return _y_totalSize; }
        set
        {
            _y_totalSize = value;
        }
    }
    #endregion
    #region Methods
    public virtual void Up()
    {
        Y_selected--;

        if (Y_selected == Y_offset - 1)
            Y_offset--;
    }
    public virtual void Down()
    {
        Y_selected++;

        if (Y_selected == Y_offset + Math.Min(Y_visible, Y_totalSize)) //Math.Min(Y_visible, Y_totalSize)
            Y_offset++;
    }

    public void PageUp()
    {
        Y_selected = Y_selected - Y_visible;
        Y_offset = Y_offset - Y_visible;
    }
    public void PageDown()
    {
        Y_selected = Y_selected + Y_visible;
        Y_offset = Y_offset + Y_visible;
    }
    public virtual void GoBegin()
    {
        Y_selected = 0;
        Y_offset = 0;
    }
    public virtual void GoEnd()
    {
        Y_selected = Y_totalSize - 1;
        Y_offset = Y_totalSize - Y_visible;
    }

    #region Component
    //public void Draw()
    //{
    //    //Console.SetCursorPosition(0, Y_selected);
    //    //Console.Write("");
    //}
    //public void HandleKey(ConsoleKeyInfo info)
    //{
    //    switch (info.Key)
    //    {
    //        case ConsoleKey.UpArrow:
    //            ScrollUp();
    //            return;
    //        case ConsoleKey.DownArrow:
    //            ScrollDown();
    //            return;
    //        case ConsoleKey.Home:
    //            GoBegin();
    //            return;
    //        case ConsoleKey.End:
    //            GoEnd();
    //            return;
    //        case ConsoleKey.PageUp:
    //            PageUp();
    //            return;
    //        case ConsoleKey.PageDown:
    //            PageDown();
    //            return;
    //    }
    //}
    #endregion
    #endregion


}
