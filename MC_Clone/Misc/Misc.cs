using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class Misc
{
}


internal class myPath
{
    public static string project(string file = "", int up = 4)
    {
        string startupPath = Environment.CurrentDirectory; // ...\Project\bin\Debug\net6.0
        string[] paths = startupPath.Split(@"\");
        string path = "";
        for (int i = 0; i < paths.Length - up; i++)
        {
            path += paths[i];
            if (i != paths.Length - (up + 1))
                path += @"\";
        }
        path += @"\" + file;

        return path;
    }

    public static string desktop(string file = "", string folder = @"")
    {
        if (folder != "")
            folder += $@"\{folder}\";

        string dekstop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string path = dekstop + folder + file;
        //@"C:\Users\user\Desktop\test.csv";

        return path;
    }

    public static void fileCheck(string path) //create file if doesnt exist
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Soubor nebyl nalezen");
            return;
        }
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
