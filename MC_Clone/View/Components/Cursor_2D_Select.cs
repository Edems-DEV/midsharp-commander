using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Cursor_2D_Select //rename to marker?
{
    //editor
    Cursor_2D Cursor;
    public List<string> Rows; //reference on original rows in FileEditor


    public bool MarkedMode = false; //true => hooked on cursor (listen for it)
    //marker pos
    int start_X;
    int start_Y;

    int leftCursor_X = 0;
    int rightCursor_X = 0;
    int leftCursor_Y = 0;
    int rightCursor_Y = 0;
    List<string> selectedRows = new List<string>();

    public int linesCout { get { return (leftCursor_Y - rightCursor_Y); } } //+ 1 (?)

    public Cursor_2D_Select(FileEditor editor)
    {
        Cursor = editor.Cursor;
        Rows = editor.Rows; //by reference => should change original List
    }

    public void Mark()
    {
        MarkedMode = !MarkedMode;

        if (MarkedMode)
            SetMarker();
    }

    public void SetMarker()
    {
        start_X = Cursor.X_selected;
        start_Y = Cursor.Y_selected;
    }

    public void Hook()
    {
        if (MarkedMode)
            Update(); //hook
    }

    public void Update() //updateSelection
    {
        if (Cursor.X_selected == start_X
         && Cursor.Y_selected == start_Y) //Marker = Cursor => return
        {
            #region Comments
            //Cursor.pos = Marker.Pos => return (only cursor) [make pos object?]
            //TODO: finish this (used at update + F3) => zero selection (right place? => no more calc)
            #endregion
            return;
        }

        SetCursorsSides();
        GetDataToMark();
        DrawMarker();
    }

    public void SetCursorsSides() //SetCursorsSides
    {
        if (start_Y == Cursor.Y_selected)
        {
            if (start_X < Cursor.X_selected) //switch sides (left arrow)
            {
                Set_Start_Left();
            }
            else
            {
                Set_Start_Right();
            }
        }
        else if (start_Y < Cursor.Y_selected)
        {
            Set_Start_Left();
        }
        else if (start_Y > Cursor.Y_selected)
        {
            Set_Start_Right();
        }
    }
    #region HelperMethod
    public void Set_Start_Right() //find way to have one method => use arguments
    {
        leftCursor_X = Cursor.X_selected;
        rightCursor_X = start_X;

        leftCursor_Y = Cursor.Y_selected;
        rightCursor_Y = start_Y;
    }
    public void Set_Start_Left()
    {
        leftCursor_X = start_X;
        rightCursor_X = Cursor.X_selected;

        leftCursor_Y = start_Y;
        rightCursor_Y = Cursor.Y_selected;
    }
    #endregion

    public void GetDataToMark()
    {
        int Y_counter = start_Y;

        if (start_Y == Cursor.Y_selected) //single row select
        {
            selectedRows.Add(Rows[Y_counter].Substring(leftCursor_X, rightCursor_X));
        }
        else
        {
            //start substring
            //full rows (if rows > 2)
            //end substring

            selectedRows.Add(Rows[Y_counter].Substring(leftCursor_X, Rows[Y_counter].Length - leftCursor_X)); //first line
            if (linesCout >= 2) //have middle full lines
            {
                //save middle lines
                while (Y_counter < linesCout - 2)
                {
                    Y_counter++;
                    selectedRows.Add(Rows[Y_counter]);
                }
            }
            selectedRows.Add(Rows[Y_counter].Substring(0, rightCursor_X)); //last row
        }
    }
    public void DrawMarker() //rename to 'Draw'?
    {
        //TODO: add line Wrapper
        int Y_counter = start_Y;

        #region SetColor
        ConsoleColor oldBG = Console.BackgroundColor;
        Console.BackgroundColor = Config.Accent_BackgroundColor; //TODO: own color in config
        #endregion

        //if (Marked) => draw always just empthy string (? performance)
        if (start_Y == Cursor.Y_selected) //single row select
        {
            DrawLineOn(Y_counter, leftCursor_X);
        }
        else
        {
            DrawLineOn(Y_counter, leftCursor_X); Y_counter++;
            if (linesCout >= 2)
            {
                while (Y_counter < linesCout - 2)
                {
                    DrawLineOn(Y_counter); Y_counter++;
                }
            }
            DrawLineOn(Y_counter);
        }
        Console.BackgroundColor = oldBG;
    }

    public void DrawLineOn(int rowIndex, int x = 0)
    {
        //TODO: add line wrapper
            // Draw in FE? => No, standalone object
            //
        Console.SetCursorPosition(rowIndex, x);
        Console.Write(Rows[rowIndex]);
    }
}
