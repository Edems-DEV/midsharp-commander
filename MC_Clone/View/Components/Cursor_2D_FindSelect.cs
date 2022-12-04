using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Cursor_2D_FindSelect //TODO: find better name
{
    #region Editor
    private FileEditor editor;
    private Cursor_2D Cursor;
    private Application Application;
    private List<string> Rows = new List<string>();
    #endregion

    public List<int> SelectedRow = new List<int>();
    private string oldSearch = "";

    public string OldSearch
    {
        get { return oldSearch; }
        set { oldSearch = value; }
    }

    public Cursor_2D_FindSelect(FileEditor editor)
    {
        this.editor = editor;
        this.Cursor = editor.Cursor;
        this.Application = editor.Application;
        this.Rows = editor.Rows;

    }

    #region Draw
    public void Draw()
    {
        Draw_SearchAll();
    }
    public void WriteSelect(string text, int x, int y) //useless?
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
    #endregion

    #region Search
    public void Search()
    {
        Application.SwitchPopUp(new File_Search(editor));
    }
    
    #region Search
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
    #endregion
    #region SearchAll
    public void Draw_SearchAll()
    {
        if (SelectedRow.Count == 0) //perfomance? => don't change color if will do nothing?
            return;

        #region Original values
        ConsoleColor oldBG = Console.BackgroundColor;
        ConsoleColor oldTEXT = Console.ForegroundColor;
        int old_Y = Console.CursorTop;
        int old_X = Console.CursorLeft;
        #endregion

        Console.BackgroundColor = ConsoleColor.Green; //TODO: move to config
        Console.ForegroundColor = Config.Selection_Foreground;

        for (int i = 0; i < SelectedRow.Count - 1; i++)
        {
            if (SelectedRow[i] < editor.Cursor.Y_offset ||
                SelectedRow[i] > editor.Cursor.Y_offset + editor.Cursor.Y_visible)
                continue;

            int a = SelectedRow[i] - Cursor.Y_offset;
            Console.SetCursorPosition(0, a + 1); //+1 = header
            Console.WriteLine(editor.Rows[SelectedRow[i]]);
        }

        #region Restore values
        Console.BackgroundColor = oldBG;
        Console.ForegroundColor = oldTEXT;
        Console.SetCursorPosition(old_X, old_Y);
        #endregion
    }
    public void Set_SearchAll(string searchedString)
    {
        oldSearch = searchedString;

        SelectedRow.Clear();
        
        for (int i = 0; i < Rows.Count; i++)
        {   
            if (Rows[i].Contains(oldSearch))
            {
                SelectedRow.Add(i);
            }
        }
    }
    #endregion
    #endregion

    #region Replace
    public void Replace()
    {
        Application.SwitchPopUp(new File_Replace());
    }
    #endregion


}
