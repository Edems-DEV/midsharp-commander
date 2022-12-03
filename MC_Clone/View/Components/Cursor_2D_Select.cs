using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
    int marker_X;
    int marker_Y;

    int leftCursor_X = 0;
    int rightCursor_X = 0;
    int leftCursor_Y = 0;
    int rightCursor_Y = 0;
    public List<string> selectedRows = new List<string>();

    public int linesCout { get { return (rightCursor_Y - leftCursor_Y); } } //+ 1 (?)
    public int spaceBetween_L_R { get { return (rightCursor_X - leftCursor_X); } } //concept

    public Cursor_2D_Select(FileEditor editor)
    {
        Cursor = editor.Cursor;
        Rows = editor.Rows; //by reference => should change original List
    }

    public void DebugPrint()
    {
        int x_selected = Cursor.X_selected;
        int y_selected = Cursor.Y_selected;
        List<string> Debug = new List<string>();
        string line1 = String.Format("  Left: X: {0}  Y: {1} |  Right: X: {2}  Y: {3}",  leftCursor_X, leftCursor_Y, rightCursor_X, rightCursor_Y);
        string line2 = String.Format("Marker: X: {0}  Y: {1} | Cursor: X: {2}  Y: {3}",  marker_X,     marker_Y,     x_selected,    y_selected);
        string line3 = String.Format("Lines: {0}", linesCout);
        Debug.Add(line1);
        Debug.Add(line2);
        Debug.Add(line3);
        Debug.Add(new String('•', Debug[Debug.Count - 1].Length));
        Debug.AddRange(selectedRows);
        Logs a = new Logs(Debug);
    }
    public void Mark()
    {
        MarkedMode = !MarkedMode;

        if (MarkedMode)
            SetMarker();
    }

    public void SetMarker()
    {
        marker_X = Cursor.X_selected;
        marker_Y = Cursor.Y_selected;
    }

    public void Hook()
    {
        if (MarkedMode)
            Update(); //hook
    }

    public void Update() //updateSelection
    {
        if (Cursor.X_selected == marker_X
         && Cursor.Y_selected == marker_Y) //Marker = Cursor => return
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
        if (marker_Y == Cursor.Y_selected)
        {
            if (marker_X < Cursor.X_selected) //switch sides (left arrow)
            {
                Set_Start_Left();
            }
            else
            {
                Set_Start_Right();
            }
        }
        else if (marker_Y < Cursor.Y_selected)
        {
            Set_Start_Left();
        }
        else if (marker_Y > Cursor.Y_selected)
        {
            Set_Start_Right();
        }
    }
    #region HelperMethod
    public void Set_Start_Right() //find way to have one method => use arguments
    {
        leftCursor_X = Cursor.X_selected;
        rightCursor_X = marker_X;

        leftCursor_Y = Cursor.Y_selected;
        rightCursor_Y = marker_Y;
    }
    public void Set_Start_Left()
    {
        leftCursor_X = marker_X;
        rightCursor_X = Cursor.X_selected;

        leftCursor_Y = marker_Y;
        rightCursor_Y = Cursor.Y_selected;
    }
    #endregion

    public void GetDataToMark()
    {
        selectedRows.Clear();
        int Y_counter = leftCursor_Y;

        if (leftCursor_Y == rightCursor_Y) //single row select
        {
            selectedRows.Add(Rows[Y_counter].Substring(leftCursor_X, rightCursor_X - leftCursor_X));
        }
        else
        {
            #region Comments
            //start substring
            //full rows (if rows > 2)
            //end substring
            #endregion

            selectedRows.Add(Rows[Y_counter].Substring(leftCursor_X, Rows[Y_counter].Length - leftCursor_X)); Y_counter++; //first line
            if (linesCout >= 2) //have middle full lines
            {
                //save middle lines
                while (Y_counter - Cursor.Y_offset <= linesCout)
                {
                    selectedRows.Add(Rows[Y_counter]); Y_counter++;
                }
            }
            selectedRows.Add(Rows[Y_counter].Substring(0, rightCursor_X)); //last row
        }
    }
    public void DrawMarker()
    {
        #region SetColor
        ConsoleColor oldBG = Console.BackgroundColor;
        Console.BackgroundColor = Config.Accent_BackgroundColor;
        #endregion
        
        int Y_counter = leftCursor_Y;
        if (leftCursor_Y == rightCursor_Y)
        {
            DrawLineOn(Y_counter, leftCursor_X);
        }
        else
        {
            DrawLineOn(Y_counter, leftCursor_X); Y_counter++;
            foreach (var row in selectedRows.Skip(1))
            {
                DrawLineOn(Y_counter, row); Y_counter++;
            }
        }

        #region ResetColor
        Console.BackgroundColor = oldBG;
        #endregion
    }

    public void DrawLineOn(int rowIndex, string row, int x = 0)
    {
        int old_X = Console.CursorLeft;
        int old_y = Console.CursorTop;

        Console.SetCursorPosition(x, rowIndex + 1 - Cursor.Y_offset);
        Console.Write(row);

        Console.SetCursorPosition(old_X, old_y);
    }

    public void DrawLineOn(int rowIndex, int x = 0)
    {
        //TODO: add line wrapper
        // Draw in FE? => No, standalone object

        DrawLineOn(rowIndex, selectedRows[rowIndex - leftCursor_Y], x);
    }

    #region Functions
    //TODO: check if selection is avaible
    public void Delete()
    {
        int Y_counter = leftCursor_Y;

        if (leftCursor_Y == rightCursor_Y)
        {
            Rows[Y_counter] = Rows[Y_counter].Substring(leftCursor_X, rightCursor_X - leftCursor_X);
        }
        else
        {
            Rows[Y_counter] = Rows[Y_counter].Substring(leftCursor_X, Rows[Y_counter].Length - leftCursor_X); Y_counter++;
            if (linesCout >= 2)
            {
                while (Y_counter - Cursor.Y_offset <= linesCout)
                {
                    Rows.RemoveAt(Y_counter); //Y_counter++;
                }
            }
            Rows[Y_counter] = Rows[Y_counter].Substring(0, rightCursor_X);
        }
    }
    public void Copy()
    {
        int Y_counter = leftCursor_Y;

        if (leftCursor_Y == rightCursor_Y)
        {
            #region First line
            Rows[Cursor.Y_selected] =
                Rows[Cursor.Y_selected].Substring(0, Cursor.X_selected)
                + selectedRows[0] +
                Rows[Cursor.Y_selected].Substring(Cursor.X_selected, Rows[Cursor.Y_selected].Length - Cursor.X_selected); 
            #endregion
        }
        else
        {
            #region First line
            Rows[Cursor.Y_selected] =
                Rows[Cursor.Y_selected].Substring(0, Cursor.X_selected)
                + selectedRows[0]; Y_counter++;
            #endregion

            selectedRows.RemoveAt(0);
            selectedRows.Reverse();
            foreach (var row in selectedRows)
            {
                Rows.Insert(Y_counter, row); Y_counter++;
            }
            Rows[selectedRows.Count - 1] = 
                selectedRows[selectedRows.Count - 1] 
                + Rows[Cursor.Y_selected].Substring(Cursor.X_selected, Rows[Cursor.Y_selected].Length - Cursor.X_selected);
        }
    }
    
    public void Move()
    {
        Delete();
        Copy();
    }


    #endregion
}
