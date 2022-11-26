using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class Cursor_1D //: IComponent //(Y '|')
{
    #region Atributes
    //RENAME
    //Selected;          // Y_selected
    //offset;            // Y_offset
    //Y_visible;         // Y_visible
    //rows.count;        // Y_totalSize
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
    protected int _y_totalSize;    // rows.count

    //Local
    protected int _y_selected = 0; // current cursor pos
    protected int _y_offset = 0;   // offset
    #endregion
    #region Properties 
    public int Y_selected
    {
        get { return _y_selected; }
        set
        {
            if (value < 0) { return; }
            //if (value >= Rows.Count) { return; }
            _y_selected = value;
        }
    }
    public int Y_offset
    {
        get { return _y_offset; }
        set
        {
            _y_offset = value;
        }
    }
    public int Y_visible
    {
        get { return _y_visible; }
        set
        {
            if (value < 0) { Y_offset = 0; return; }
            //if (Rows.Count <= Visible) { return; }
            //if (value >= Rows.Count - Visible) { _offset_X = Rows.Count - Visible; _selected = Rows.Count - 1; return; }
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
    protected void ScrollUp()
    {
        Y_selected--;

        if (Y_selected == Y_offset - 1)
            Y_offset--;
    }
    protected void ScrollDown()
    {
        Y_selected++;

        if (Y_selected == Y_offset + Math.Min(Y_visible, Y_totalSize))
            Y_offset++;
    }

    protected void PageUp()
    {
        Y_selected = Y_selected - Y_visible;
        Y_offset = Y_offset - Y_visible;
    }
    protected void PageDown()
    {
        Y_selected = Y_selected + Y_visible;
        Y_offset = Y_offset + Y_visible;
    }
    protected virtual void GoBegin()
    {
        Y_selected = 0;
        Y_offset = 0;
    }
    protected virtual void GoEnd()
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
    //            break;
    //        case ConsoleKey.DownArrow:
    //            ScrollDown();
    //            break;
    //        case ConsoleKey.Home:
    //            GoBegin();
    //            break;
    //        case ConsoleKey.End:
    //            GoEnd();
    //            break;
    //        case ConsoleKey.PageUp:
    //            PageUp();
    //            break;
    //        case ConsoleKey.PageDown:
    //            PageDown();
    //            break;
    //    }
    //}
    #endregion
    #endregion


}
