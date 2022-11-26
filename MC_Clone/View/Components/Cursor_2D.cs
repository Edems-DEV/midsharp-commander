using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class Cursor_2D : Cursor_1D //(1D + X ['-'] )
{
    public Cursor_2D(int y_start, int maxHeight, int rowsCount) : base(y_start, maxHeight, rowsCount)
    {

    }

    #region Atributes
    //Feed me
    int maxWidth;

    //   '-'
    protected int _x = 0;          // current cursor pos
    protected int _x_offset = 0;   // offset
    protected int _x_visible = 0;  // columns / maxWidth
    protected int _x_start = 0;    // original pos (for reset)
    #endregion

    #region Properties
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
    #endregion
}
