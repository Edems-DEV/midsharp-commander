using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Cursor_2D : Cursor_1D //(1D + X ['-'] )
{
    public Cursor_2D(int x_start, int maxHeight, int rowsCount) : base(x_start, maxHeight, rowsCount)
    {

    }

     #region Atributes
    //Feed me
    protected int _x_start;        // original pos (for reset)
    protected int _x_visible;      // maxHeight
    protected int _x_totalSize;    // rows.count

    //Local
    protected int _x_selected = 0; // current cursor pos
    protected int _x_offset = 0;   // offset
    #endregion
    #region Properties 
    public int X_selected
    {
        get { return _x_selected; }
        set
        {
            if (value < 0) { return; }
            //if (value >= Rows.Count) { return; }
            _x_selected = value;
        }
    }
    public int X_offset
    {
        get { return _x_offset; }
        set
        {
            _x_offset = value;
        }
    }
    public int X_visible
    {
        get { return _x_visible; }
        set
        {
            if (value < 0) { X_offset = 0; return; }
            //if (Rows.Count <= Y_visible) { return; }
            //if (value >= Rows.Count - Y_visible) { _offset_X = Rows.Count - Y_visible; _selected = Rows.Count - 1; return; }
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
        get { return _x_totalSize; }
        set
        {
            _x_totalSize = value;
        }
    }
    #endregion
    #region Methods
    protected void Up()
    {
        X_selected--;

        if (X_selected == X_offset - 1)
            X_offset--;
    }
    protected void Down()
    {
        X_selected++;

        if (X_selected == X_offset + Math.Min(X_visible, X_totalSize))
            X_offset++;
    }

    protected void PageUp()
    {
        X_selected = X_selected - X_visible;
        X_offset = X_offset - X_visible;
    }
    protected void PageDown()
    {
        X_selected = X_selected + X_visible;
        X_offset = X_offset + X_visible;
    }
    protected virtual void GoBegin()
    {
        X_selected = 0;
        X_offset = 0;
    }
    protected virtual void GoEnd()
    {
        X_selected = X_totalSize - 1;
        X_offset = X_totalSize - X_visible;
    }
    #endregion
}
