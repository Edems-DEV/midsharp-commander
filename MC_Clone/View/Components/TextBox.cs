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

    public char PadChar { get; set; } = ' ';

    public void Draw()
    {
        Console.WriteLine(this.Label);
        Console.WriteLine(PadChar + this.Value.PadRight(this.Size, PadChar));
        Console.WriteLine();
    }

    public int Draw(int x, int y)
    {
        ConsoleColor oldTextC = Console.ForegroundColor;
        ConsoleColor oldBackC = Console.BackgroundColor;
        

        Console.SetCursorPosition(x, y);
        Console.Write(Label);
        Console.ForegroundColor = Config.PopUp_ForeGroud;
        Console.BackgroundColor = ConsoleColor.Cyan; //TODO: Change me to a config value //static painter class
        Console.SetCursorPosition(x, y += 1);
        Console.Write(PadChar + Value.PadRight(Size, PadChar));

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
