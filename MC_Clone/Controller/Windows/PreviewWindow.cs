using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class PreviewWindow : Window
{
    private int rows = Console.WindowWidth;
    private int columns = Console.WindowWidth;
    private FileSystemInfo file;
    private int x;

    private int _offset = 0;
    private int _selected = 0;
    private int _visible = 10;

    public PreviewWindow(Application application, FileInfo file)
    {
        Application = application;
        this.file = file;
        Application.WinSize.OnWindowSizeChange += OnResize;
    }
        
    public override void Draw()
    {
        x++;
        Console.SetCursorPosition(0, 0);
        //for (int i = 0; i < rows - 1; i++)
        //{
        //    Console.Write(file.FullName);
        //    Console.WriteLine("-" + x);
        //}
        
    }

    public override void HandleKey(ConsoleKeyInfo info)
    {
        if (info.Key == ConsoleKey.Q)
        {
            Application.SwitchWindow(new ListWindow(Application));
        }
    }

    public void OnResize()
    {
        Console.Clear();
        
        rows = Console.WindowWidth; 
        rows -= 10; // - (header + footer)
        columns = Console.WindowWidth;
        
        Draw();
    }
}
