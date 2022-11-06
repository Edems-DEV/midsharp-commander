using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class MenuBar : IComponent
{
    public string[] labels = {"Left", "File", "Command", "Options", "Right" };
    public bool active = false;
    bool dropDownMenu = false;
    public void Draw()
    {
        int length = 0;
        int top = Console.WindowHeight;
        int rowLength = Console.WindowWidth;
        Console.SetCursorPosition(0, 0);

        foreach (var label in labels)
        {
            Console.BackgroundColor = Config.Labels_BackgroundColor;
            Console.ForegroundColor = Config.Labels_ForegroundColor;
            //if (label.active)
            //{
            //    Console.BackgroundColor = Config.Black;
            //    Console.ForegroundColor = Config.White;
            //}
            string a = $"    {label}      "; //TODO: dynamic -> smaller window
            length += a.Length;
            Console.Write(a);
        }
        if (Console.WindowWidth > length) { 
            string space = new String(' ', Console.WindowWidth - length);
            Console.Write(space);
        }
        Console.ResetColor();
    }

    public void HandleKey(ConsoleKeyInfo info) //F9 - PullOn
    {
        if (info.Key == ConsoleKey.Escape)
            active = false;

        if (!dropDownMenu) {
            switch (info.Key)
            {
                case ConsoleKey.RightArrow:
                    break;
                case ConsoleKey.LeftArrow:
                    break;
                case ConsoleKey.Enter:
                    break;
                //DropDown menu
                case ConsoleKey.UpArrow:
                    break;
                case ConsoleKey.DownArrow:
                    break;
            }
        }
        else //DropDownMenu
        {
            switch (info.Key)
            {
                case ConsoleKey.RightArrow:
                    break;
                case ConsoleKey.LeftArrow:
                    break;
                case ConsoleKey.Enter:
                    break;
                //DropDown menu
                case ConsoleKey.UpArrow:
                    break;
                case ConsoleKey.DownArrow:
                    break;
            }
        }
    }
}
