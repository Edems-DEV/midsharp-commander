using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

internal class EditWindow : Window
{
    private FileSystemInfo file;
    public FileEditor editor;
    private Header_Edit header;
    private Footer footer;
    public string[] labels;

    public EditWindow(Application application, FileInfo file)
    {
        this.Application = application;
        this.file = file;

        editor = new FileEditor(Application, file, 0, 1);
        header = new Header_Edit(editor);
        labels = new string[] { "Help", "Save", "Mark", "Replace", "Copy", "Move", "Search", "Delete", "PullDn", "Quit" };
        footer = new Footer(labels);

        Application.WinSize.OnWindowSizeChange += OnResize;
    }

    public override void Draw()
    {
        Console.Clear(); //editor
        Console.SetCursorPosition(0, 0);
        header.Draw(); //own header
        editor.Draw();
        footer.Draw();
    }

    public override void HandleKey(ConsoleKeyInfo info)
    {
        editor.HandleKey(info);
    }

    public void OnResize() //add to interface and call only in app?
    {
    }


}
public class Header_Edit //mak emore genral an make one specific for this class
{
    int width = Console.WindowWidth;
    FileEditor editor;

    int charTo;
    int charAll;

    public Header_Edit(FileEditor editor)
    {
        this.editor = editor;
        int width = Console.WindowWidth;
    }

    public void Draw()
    {
        ConsoleColor oldB = Console.BackgroundColor;
        ConsoleColor oldT = Console.ForegroundColor;
        Console.BackgroundColor = Config.Labels_BackgroundColor;
        Console.ForegroundColor = Config.Labels_ForegroundColor;

        string space = new String(' ', Console.WindowWidth);
        Console.SetCursorPosition(0, 0);
        Console.Write(space);
        CalcChars();

        string path = editor.File.Name;
        string mod = Mode();
        string pos = $"{editor.Cursor.X_selected + editor.Cursor.X_offset} L: [{editor.Cursor.Y_offset} + {editor.Cursor.Y_selected}  {editor.Cursor.Y_offset + editor.Cursor.Y_selected}/{editor.Rows.Count}]";
        string idk = $"({charTo}/{charAll}b)";
        string asci = $"{ConvertCharToAsci(editor.Cursor.input)}  {ConvertCharToAsciHex(editor.Cursor.input)}";
        string final = $"{path}  {mod}  {pos} *{idk} {asci}";
        Console.SetCursorPosition(0, 0);
        Console.Write(final);
        string debug = $" = {editor.Cursor.input}";
        Console.Write(debug);

        Console.BackgroundColor = oldB;
        Console.ForegroundColor = oldT;
    }

    public void CalcChars()
    {
        charTo = 0;
        charAll = 0;
        int i = 0;
        
        foreach (string row in editor.Rows)
        {
            if (i < editor.Cursor.Y_offset + editor.Cursor.Y_selected)
            {
                charTo += row.Length;
            }
            else if(i == editor.Cursor.Y_offset + editor.Cursor.Y_selected)
            { charTo += editor.Cursor.X_selected + editor.Cursor.X_offset; }
            charAll += row.Length;
            
            i++;
        }
    }

    public static string ConvertCharToAsci(char a)
    {
        int asciCode = (int)Convert.ToChar(a);
        return asciCode.ToString().PadLeft(4, '0');
    }
    public static string ConvertCharToAsciHex(char a)
    {
        string hex = Convert.ToByte(a).ToString("x2");
        return "0x" + hex.ToString().PadLeft(3, '0'); ;
    }
    public string Mode(string mod = "-") //[-M---]
    {
        if (!editor.ContentChanged()) //TODO: broken on large text
        {
            mod = "M";
        }
        
        return $"[-{mod.PadRight(3,'-')}]";
    }
}
