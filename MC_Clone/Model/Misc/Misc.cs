using System.Runtime.InteropServices;

namespace MC_Clone;
internal class Misc //koncepty
{
    public static string ConvertCharToAsci(char a) //myString or return int/hex
    {
        int asciCode = (int)Convert.ToChar(a);
        return asciCode.ToString().PadLeft(4, '0');
    }
    public static string ConvertCharToAsciHex(char a)
    {
        string hex = Convert.ToByte(a).ToString("x2");
        return "0x" + hex.ToString().PadLeft(3, '0'); ;
    }

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

    public static string GetPath(string path, int cut = 1)
    {
        List<string> pathList = path.Split('\\').ToList();
        for (int i = 0; i < cut; i++)
        {
            pathList.RemoveAt(pathList.Count - 1);
        }
        string newPath = string.Join("\\", pathList);
        return newPath;
    }
    
    public static bool IsIlegalChar(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            case ConsoleKey.Enter:
            case ConsoleKey.Tab:
                return true;
        }
        return false;
    }
    public static bool IsNumber(ConsoleKeyInfo info)
    {
        switch (info.Key)
        {
            case ConsoleKey.D0:
            case ConsoleKey.D1:
            case ConsoleKey.D2:
            case ConsoleKey.D3:
            case ConsoleKey.D4:
            case ConsoleKey.D5:
            case ConsoleKey.D6:
            case ConsoleKey.D7:
            case ConsoleKey.D8:
            case ConsoleKey.D9:
                return true;
        }
        return false;
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
    public enum Ć
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
