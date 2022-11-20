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
        ConsoleColor oldBack = Console.BackgroundColor;
        ConsoleColor oldText = Console.ForegroundColor;

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
            Console.BackgroundColor = Config.Cout_BackgroundColor;
            Console.ForegroundColor = Config.Cout_ForegroundColor;
            Console.Write(number.PadLeft(numberPad));

            Console.BackgroundColor = Config.Labels_BackgroundColor;
            Console.ForegroundColor = Config.Labels_ForegroundColor;
            Console.Write(label.PadRight(totalPad));

            count++;
        }
        Console.BackgroundColor = oldBack;
        Console.ForegroundColor = oldText;
        Console.CursorVisible = false;
        Console.SetCursorPosition(0,0);
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
    }
}
