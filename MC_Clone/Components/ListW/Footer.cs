using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class Footer : IComponent
{
    public string[] labels { get; set; }

    public Footer(string[] Labels)
    {
        labels = Labels;
    }

    public void Draw()
    {
        int top = Console.WindowHeight;
        int rowLength = Console.WindowWidth;
        Console.SetCursorPosition(0, top - 1); // -1 (last visible row) - 1 (buggy scrool fix -> from readkey?)

        int numberPad = 2;

        int numberSpace = labels.Length * numberPad;
        int totalPad = ((rowLength - numberSpace) / labels.Length);

        int count = 1;
        foreach (var label in labels)
        {
            string number = count.ToString();
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(number.PadLeft(numberPad));

            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(label.PadRight(totalPad));

            count++;
        }
        Console.ResetColor();
        Console.CursorVisible = false;
        Console.SetCursorPosition(0,0);
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
    }
}

internal class DebugConsole : IComponent
{
    int ySize;
    int localY = 0;
    public DebugConsole(int y)
    {
        localY = y;
    }
    public void CleanLine()
    {
        Console.SetCursorPosition(0, Console.WindowHeight - localY);
        int winWidth = Console.WindowWidth;
        string space = new String(' ', winWidth);
        Console.Write(space);
    }
    public void Log(string msg)
    {
        CleanLine();
        Console.SetCursorPosition(0, Console.WindowHeight - localY);
        msg = $" > {msg}";
        Console.Write(msg);
    }

    public void Draw()
    {
        Log("");
    }
    public void HandleKey(ConsoleKeyInfo info)
    {
    }
}
