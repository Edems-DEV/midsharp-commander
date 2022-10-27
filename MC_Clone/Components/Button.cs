using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class Button : IComponent
{
    public event Action Clicked;

    public string Title { get; set; } = "";

    public void Draw()
    {
        Console.WriteLine($"[ {this.Title} ]");
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
        if (info.Key == ConsoleKey.Enter)
        {
            this.Clicked();
        }
    }
}
