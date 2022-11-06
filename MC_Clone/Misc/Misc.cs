using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class Misc //koncepty
{
    public static string PadBoth(string source, int length)
    {
        int spaces = length - source.Length;
        int padLeft = spaces / 2 + source.Length;
        if (length < 0)
            return source;
        return source.PadLeft(padLeft).PadRight(length);

    }
    public static void ClearConsole()
    {
        Console.BackgroundColor = Config.Primary_BackgroundColor; //paint whole console
        Console.Clear();
        Console.SetCursorPosition(0, 0);
    }
}


internal class ConsoleBTN
{
    private const int MF_BYCOMMAND = 0x00000000;
    public const int SC_CLOSE = 0xF060;
    public const int SC_MINIMIZE = 0xF020;
    public const int SC_MAXIMIZE = 0xF030;

    [DllImport("user32.dll")]
    public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    public static void disable_CloseBTN()
    {
        DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
    }
    public static void disable_MaximizeBTN()
    {
        DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
    }
    public static void disable_MinimizeBTN()
    {
        DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND); 
    }
}

internal class MyColor
{
    [Flags]
    public enum Dockings
    {
        Accent = 0,
        Primary = 1,
        Secondary = 2,
    }
    
    public static void Recolor(string type, string text) //idea - bad
    {
        ConsoleColor t = Console.ForegroundColor;
        ConsoleColor b = Console.BackgroundColor;

        switch (type)
        {
            case "primary":
                t = Config.Primary_ForegroundColor;
                b = Config.Primary_ForegroundColor;
                break;
            default:
                break;
        }

        ConsoleColor old_TextColor = Console.ForegroundColor;
        ConsoleColor old_BackgroudColor = Console.BackgroundColor;
        //just 
        Console.ForegroundColor = t;
        Console.BackgroundColor = b;
        Console.WriteLine(text);
        Console.ForegroundColor = old_TextColor;
        Console.BackgroundColor = old_BackgroudColor;
    }
}
