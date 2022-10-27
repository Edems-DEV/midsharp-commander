using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Application
{
    private Window window;

    public Application()
    {
        this.SwitchWindow(new ListWindow());
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
        this.window.HandleKey(info);
    }

    public void Draw()
    {
        this.window.Draw();
    }

    public void SwitchWindow(Window window)
    {
        window.Application = this;
        this.window = window;

        Console.Clear();
    }
}
