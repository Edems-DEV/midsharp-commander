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
        //Console.WriteLine($"[ {this.Title} ]");
        var a = Console.ForegroundColor;
        int asscentLetters = 1;

        Console.Write("[ ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        
        Console.Write(this.Title.Substring(0, asscentLetters));
        Console.ForegroundColor = a;
        Console.Write(this.Title.Substring(asscentLetters, this.Title.Length - asscentLetters));
        Console.Write($" ]");
        //Console.WriteLine();
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
        if (info.Key == ConsoleKey.Enter)
        {
            this.Clicked();
        }
    }
}
