using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class PreviewWindow : Window
{
    private FileSystemInfo file;
    private FileEditor editor;
    private Header header;
    private Footer footer;

    public PreviewWindow(Application application, FileInfo file)
    {
        this.Application = application;
        this.file = file;

        editor = new FileEditor(Application, file, 0, 1);
        header = new Header(file);
        string[] labels = { "Help", "UnWrap", "Quit", "Hex", "Goto", "    ", "Search", "Raw", "Format", "Quit" };
        footer = new Footer(labels);

        Application.WinSize.OnWindowSizeChange += OnResize;
    }
        
    public override void Draw()
    {
        Console.Clear(); //editor
        Console.SetCursorPosition(0, 0);
        header.Draw();
        editor.Draw();
        footer.Draw();
    }

    public override void HandleKey(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            case ConsoleKey.Escape:
            case ConsoleKey.F3:
            case ConsoleKey.F10:
                Quit();
                return;
        }

        editor.HandleKey(info);
    }

    public void OnResize() //add to interface and call only in app?
    {
    }

    public void Quit()
    {
        Application.SwitchWindow(new ListWindow(Application));
    }

}

public class Header //mak emore genral an make one specific for this class
{
    int width = Console.WindowWidth;
    FileInfo file;

    public Header(FileInfo file)
    {
        this.file = file;
        int width = Console.WindowWidth;
    }

    public void Draw()
    {
        string path = file.FullName;
        path = path.PadRight(width, ' ');
        Console.Write(path);

        // Write(10, "100/100");
    }

    public void Write(int rightOffset, string text)
    {
        if (rightOffset > width)
        {
            throw new Exception($"({rightOffset}) Right Offset > Width ({width})");
        }
        int pos = width - rightOffset;
        Console.SetCursorPosition(0, pos);
        Console.Write(text);
    }

    #region Bad Idea
    string line;
    List<string> values = new List<string>();
    //public void Header() //TODO: change to own Class/Object
    //{
    //    int spaceCount = 10;
    //    int charCount = 1000;
    //    int percentageViewed = 5;

    //    string space = new String(' ', spaceCount); ;
    //    string text = ""; //TODO: change to string builder
    //    text += file.FullName;
    //    text += space;
    //    text += $"{charCount - 200}/{charCount}"; //only for showcase
    //    text += space;
    //    text += $"{percentageViewed}%";
    //    Console.WriteLine(text);
    //}

    //public void AddHeaderText(string text)
    //{
    //    if (text.Length > width)
    //        throw new Exception($"Text is tool large | Text: {text.Length} / Width: {width}");
    //    width -= text.Length;
    //    values.Add(text);
    //}
    #endregion
}
