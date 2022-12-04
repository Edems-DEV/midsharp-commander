using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Cursor_2D_FindSelect //TODO: find better name
{
    FileEditor editor;
    Cursor_2D Cursor;
    Application Application;
    List<string> Rows = new List<string>();

    string oldSearch = "";
    List<int> SelectedRow = new List<int>();

    public Cursor_2D_FindSelect(FileEditor editor)
    {
        this.editor = editor;
        this.Cursor = editor.Cursor;
        this.Application = editor.Application;
        this.Rows = editor.Rows;

    }

    public void Search()
    {
        Application.SwitchPopUp(new File_Search(editor));
    }
    public void Replace()
    {
        Application.SwitchPopUp(new File_Replace());
    }
    public void SearchString(string searchString, int lastSearchIndex = 0)
    {
        for (int i = lastSearchIndex + 1; i < Rows.Count; i++)
        {
            if (Rows[i].Contains(searchString))
            {
                //todo add offest
                Cursor.X_selected = Rows[i].IndexOf(searchString);
                if (i > Cursor.Y_visible)
                {
                    Cursor.Y_offset = i - Cursor.Y_visible;
                }

                Cursor.Y_selected = i - Cursor.Y_offset;
                WriteSelect(searchString, Cursor.X_selected, Cursor.Y_selected);
                return;
            }
        }
        Application.SwitchPopUp(new NotFound("Search", "Search string not found"));
    }

    public void WriteSelect(string text, int x, int y)
    {
        #region Original values
        ConsoleColor oldBG = Console.BackgroundColor;
        ConsoleColor oldTEXT = Console.ForegroundColor;
        int old_Y = Console.CursorTop;
        int old_X = Console.CursorLeft;
        #endregion

        Console.BackgroundColor = Config.Selection_Backgroud;
        Console.ForegroundColor = Config.Selection_Foreground;

        Console.SetCursorPosition(x, y + 1);//+1 => header
        Console.Write(text);

        #region Restore values
        Console.BackgroundColor = oldBG;
        Console.ForegroundColor = oldTEXT;
        Console.SetCursorPosition(old_X, old_Y);
        #endregion
    }
}
