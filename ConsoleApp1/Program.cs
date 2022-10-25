using System.Globalization;

namespace ConsoleApp1;

internal class Program
{
    char down = '│';
    string path = @"C:\Users\root\Desktop\x";


    static void Main(string[] args)
    {
        //Console.WriteLine("".PadRight(40, '─'));
        //active(true);
        //cross(40);
        //Console.WriteLine("".PadRight(40, '─'));
        //freeSpace();
        //files();
        //upDir();
        dir();


    }
    static void dir() //prefix '/'
    {
        DirectoryInfo directory = new DirectoryInfo(@"C:\Users\root\Desktop\x");
        DirectoryInfo[] directories = directory.GetDirectories();

        foreach (DirectoryInfo folder in directories)
            Console.WriteLine(folder.Name);
    }
    static void upDir()
    {
        DirectoryInfo d = new DirectoryInfo(@"C:\Users\root\Desktop\x");
        string date = d.LastWriteTime.ToString("MMM dd HH:mm");
        string[] a = { "/..", "UP--DIR", date};
        string aa = $"/.. | UP--DIR | {date}";
        Console.WriteLine(aa);
    }
    static void files()
    {
        DirectoryInfo d = new DirectoryInfo(@"C:\Users\root\Desktop\x");
        FileInfo[] Files = d.GetFiles();
        var Row = new List<string[]>();
        const int KB = 1024;
        foreach (var item in Files)
        {
            //CultureInfo en = new CultureInfo("en-US"); //english - ("MMM dd HH:mm", en)
            string[] a = { item.Name, (item.Length / KB).ToString(), item.LastWriteTime.ToString("MMM dd HH:mm") };
            string aa = $"{item.Name} | {(item.Length / KB).ToString()} | {item.LastWriteTime.ToString("MMM dd HH:mm")}";
            Console.WriteLine(aa);
        }
    }

    static void active(bool active)
    {
        int x = Console.CursorLeft;
        int y = Console.CursorTop;
        Console.CursorLeft = 0;
        Console.CursorTop = 0;
        Console.Write("┌<─");
        if (active)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        Console.Write($"  ~  ");
        
        //reset
        Console.ResetColor();
        Console.CursorLeft = x;
        Console.CursorTop = y;
    }
    static void cross(int paneLength)
    {
        int x = Console.CursorLeft;
        int y = Console.CursorTop;
        
        string line = ".[^]>┐";
        Console.CursorLeft = paneLength - line.Length;
        Console.CursorTop = 0;
        Console.Write(line);

        Console.CursorLeft = x;
        Console.CursorTop = y;
    }

    static void freeSpace(int lastRow = 0, int paneLength = 50) // 52G/58G (89%)
    {
        int x = Console.CursorLeft;
        int y = Console.CursorTop;
        
        Console.CursorTop = lastRow + 1;

        const double GB = 1073741824; //bytesToGb

        DriveInfo[] allDrives = DriveInfo.GetDrives();
        
        
        foreach (var item in allDrives)
        {
            double freeSpacePerc = Math.Round((item.AvailableFreeSpace / (float)item.TotalSize) * 100, 0);
            int Total = Convert.ToInt32(item.TotalSize / GB);
            int Free = Convert.ToInt32(item.TotalFreeSpace / GB);
            
            string name = $" {item.Name}: {Free}G/{Total}G ({freeSpacePerc}%) ";
            Console.CursorLeft = paneLength - name.Length - 2;
            Console.WriteLine(name);
        }


        Console.CursorLeft = x;
        Console.CursorTop = y;
    }
}