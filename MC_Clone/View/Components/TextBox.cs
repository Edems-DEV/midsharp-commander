using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MC_Clone;

public class IntBox : TextBox
{
    public override void HandleKey(ConsoleKeyInfo info)
    {
        if (info.Key == ConsoleKey.Backspace)
        {
            if (this.Value.Length == 0)
                return;

            this.Value = this.Value.Substring(0, this.Value.Length - 1);
        }
        else
        {
            if (Misc.IsNumber(info)) //filter property?
            {
                this.Value += info.KeyChar;
            }
        }
    }
}
public class TextBox : IComponent
{
    public string Label { get; set; } = "";

    public string Value { get; set; } = "";
    public string DisplayValue { get; set; } = "";

    public int Size { get; set; } = 20;
    private int size = 20;

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
        Console.BackgroundColor = Config.TextBox_InputField;
        Console.SetCursorPosition(x, y += 1);
        Console.Write(LineMover());

        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;
        int ySize = 2;
        return ySize;
    }

    public string LineMover()
    {
        string value = Value;
        if (Value.Length > size) //comment out for size to be dynamic / (disable line mover)
        {
            int start = (Value.Length - size) + 1;
            value = Value.Substring(start, size - 1);
            Size = size;
        }
        DisplayValue = value;
        value = PadChar + value.PadRight(Size, PadChar);
        return value;
    }
    
    public virtual void HandleKey(ConsoleKeyInfo info)
    {
        if (info.Key == ConsoleKey.Backspace)
        {
            if (this.Value.Length == 0)
                return;

            this.Value = this.Value.Substring(0, this.Value.Length - 1);
        }
        else
        {
            if (!Misc.IsIlegalChar(info))
            {
                this.Value += info.KeyChar;
            }
        }
    }
}
