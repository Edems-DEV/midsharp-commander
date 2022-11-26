using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class EditWindow : Window
{
    private FileSystemInfo file;
    public FileEditor editor;
    private Header header;
    private Footer footer;
    public string[] labels;

    public EditWindow(Application application, FileInfo file)
    {
        this.Application = application;
        this.file = file;

        editor = new FileEditor(Application, file, 0, 1);
        header = new Header(file);
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
        switch (info.Key)
        {
            case ConsoleKey.Escape:
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
