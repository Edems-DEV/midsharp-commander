using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Cursor_2D_Select //rename to marker?
{
    FileEditor Editor;
    public bool Logs = true;
    //editor
    Cursor_2D Cursor;
    public List<string> Rows; //reference on original rows in FileEditor


    public bool SelectionAlive = false; //Able to draw //TODO: remove (find better solution)
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
        this.Editor = editor;
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
        string line4 = String.Format("MarkedMode: {0} | SelectionAlive {1}", MarkedMode, SelectionAlive);
        Debug.Add(line1);
        Debug.Add(line2);
        Debug.Add(line3 + " | " + line4);
        Debug.Add(new String('•', Debug[Debug.Count - 1].Length));
        Debug.AddRange(selectedRows);
        Logs a = new Logs(Debug);
    }
    public void Mark()
    {
        MarkedMode = !MarkedMode;

        if (MarkedMode)
        {
            SetMarker();
            SelectionAlive = true; //Alive 1
        }
    }

    public void SetMarker()
    {
        marker_X = Cursor.X_selected;
        marker_Y = Cursor.Y_selected;
    }

    public void Hook()
    {
        if (MarkedMode)
        {
            SetCursorsSides();
            GetDataToMark();
            
            if (Cursor.X_selected == marker_X
            && Cursor.Y_selected == marker_Y) //Marker = Cursor => return
                SelectionAlive = false;
            else
                SelectionAlive = true;
        }
        if (SelectionAlive)
        {
            DrawMarker();
        }
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
                while (Y_counter - Cursor.Y_offset - leftCursor_Y < linesCout)
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
        Console.BackgroundColor = Config.Selection_Backgroud;
        ConsoleColor oldTEXT = Console.BackgroundColor;
        Console.ForegroundColor = Config.Selection_Foreground;
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
        Console.ForegroundColor = oldTEXT;
        #endregion
    }

    public void DrawLineOn(int rowIndex, string row, int x = 0)
    {
        int old_X = Console.CursorLeft;
        int old_y = Console.CursorTop;

        //if (0 > rowIndex + 1 - Cursor.Y_offset){return;} //concept
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
    public void Delete()
    {
        if (!SelectionAlive) { return; }
        
        int Y_counter = leftCursor_Y;

        string firstLine = Rows[Y_counter].Substring(0, leftCursor_X);

        string endLine = Rows[Y_counter].Substring(rightCursor_X, Rows[Y_counter].Length - rightCursor_X);


        if (leftCursor_Y == rightCursor_Y)
        {
            Editor.Rows[Y_counter] = firstLine + endLine;
        }
        else
        {
            Editor.Rows[Y_counter] = firstLine + endLine; Y_counter++;
            if (linesCout >= 2)
            {
                while (Y_counter - Cursor.Y_offset - leftCursor_Y < linesCout + 1)
                {
                    Editor.Rows.RemoveAt(Y_counter); Y_counter++;
                }
            }
        }
        SelectionAlive = false;
        MarkedMode = false;
    }
    public void Copy()
    {
        //if (!SelectionAlive) { return; }
        
        int Y_counter = Cursor.Y_selected;

        string firstLine = Rows[Cursor.Y_selected].Substring(0, Cursor.X_selected);
        
        string endLine = Rows[Cursor.Y_selected].Substring(Cursor.X_selected, Rows[Cursor.Y_selected].Length - Cursor.X_selected);

        if (selectedRows.Count  == 1)
        {
            Editor.Rows[Cursor.Y_selected] = firstLine + selectedRows[0] + endLine;
        }
        else
        {
            Editor.Rows[Cursor.Y_selected] = firstLine + selectedRows[0]; Y_counter++;

            selectedRows.RemoveAt(0);
            selectedRows.Reverse();
            foreach (var row in selectedRows)
            {
                Editor.Rows.Insert(Y_counter, row); Y_counter++;
            }
            Editor.Rows[Y_counter - 1] = selectedRows[selectedRows.Count - 1] + endLine;
        }
        
        if (Logs){
            Logs a = new Logs(Rows[Cursor.Y_selected], "Marker/COPY");
        }
    }
    
    public void Move()
    {
        Copy();
        Delete();
    }


    #endregion
}
