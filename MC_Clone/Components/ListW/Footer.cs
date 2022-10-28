using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class Footer : IComponent
{
    //public List<string> labels { get; set; }
    public string[] labels { get; set; }

    //= { "Help", "Menu", "View", "Edit", "Copy", "RenMov", "Mkdir", "Delete", "PullDn", "Quit" };
    public Footer(string[] Labels)
    {
        labels = Labels;
        //labels = new List<string> { "Help", "Menu", "View", "Edit", "Copy", "RenMov", "Mkdir", "Delete", "PullDn", "Quit" };
    }

    public void Draw()
    {
        int top = Console.WindowHeight;
        int rowLength = Console.WindowWidth;
        Console.SetCursorPosition(0, top - 1 - 1); //last visible row - 1 (buggy scrool fix)

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
        Console.WriteLine();
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
    }
}
