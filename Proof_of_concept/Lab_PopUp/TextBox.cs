using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_PopUp;
public class TextBox : IComponent
{
    public string Label { get; set; } = "";

    public string Value { get; set; } = "";

    public int Size { get; set; } = 20;

    public void Draw()
    {
        Console.Write(this.Label);
        Console.Write("_" + this.Value.PadRight(this.Size, '_'));
        //Console.WriteLine();
    }
    public int  Draw(int x, int y)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(this.Label);
        Console.SetCursorPosition(x, y++);
        Console.Write("_" + this.Value.PadRight(this.Size, '_'));

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