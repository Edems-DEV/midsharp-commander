using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class TextBox : IComponent
{
    public string Label { get; set; } = "";

    public string Value { get; set; } = "";

    public int Size { get; set; } = 20;

    public void Draw()
    {
        Console.WriteLine(this.Label);
        Console.WriteLine("_" + this.Value.PadRight(this.Size, '_'));
        Console.WriteLine();
    }

    public int Draw(int x, int y)
    {
        ConsoleColor oldTextC = Console.ForegroundColor;
        ConsoleColor oldBackC = Console.BackgroundColor;
        

        Console.SetCursorPosition(x, y);
        Console.Write(Label);
        Console.ForegroundColor = ConsoleColor.Black; //Config.PopUp_ForeGroud;
        Console.BackgroundColor = ConsoleColor.Cyan; //TODO: Change me to a config value //static painter class
        Console.SetCursorPosition(x, y += 1);
        Console.Write("_" + Value.PadRight(Size, '_'));

        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;
        int ySize = 2;
        return ySize;
    }
    
    public void HandleKey(ConsoleKeyInfo info)
    {
        if (info.Key == ConsoleKey.Backspace)
        {
            if (this.Value.Length == 0)
                return;

            this.Value = this.Value.Substring(0, this.Value.Length - 1);
        }
        else
        {
            this.Value += info.KeyChar;
        }
    }
}
