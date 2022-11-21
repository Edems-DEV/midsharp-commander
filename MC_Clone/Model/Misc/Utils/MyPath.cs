using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

internal class MyPath
{
    public static string Project(string file = "", int up = 4)
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

    public static string Desktop(string file = "", string folder = @"")
    {
        if (folder != "")
            folder += $@"\{folder}\";

        string dekstop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string path = dekstop + folder + file;
        //@"C:\Users\user\Desktop\test.csv";

        return path;
    }

    public static void FileCheck(string path) //create file if doesnt exist
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("Soubor nebyl nalezen");
            return;
        }
    }
}
