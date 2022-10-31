using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class FileManager
{
    private List<FileSystemInfo> FS_Objects = new List<FileSystemInfo>();
    //private bool _discs;
    //private string path = "";

    public FileManager() //can be static?
    {
    }

    //public FileManager(string Path)
    //{
    //    path = Path;
    //}



    #region Import
    public List<FileSystemInfo> SetLists(string path)
    {
        if (this.FS_Objects.Count != 0)
        {
            this.FS_Objects.Clear();
        }
        
        //_discs = false;
        DirectoryInfo levelUpDirectory = null;
        this.FS_Objects.Add(levelUpDirectory);
        //Directories
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            this.FS_Objects.Add(di);
        }
        //Files
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            FileInfo fi = new FileInfo(file);
            this.FS_Objects.Add(fi);
        }

        return this.FS_Objects;
    }
    public List<FileSystemInfo> SetDiscs()
    {
        if (this.FS_Objects.Count != 0)
        {
            this.FS_Objects.Clear();
        }

        //_discs = true;
        DriveInfo[] discs = DriveInfo.GetDrives();
        foreach (DriveInfo disc in discs)
        {
            if (disc.IsReady)
            {
                DirectoryInfo di = new DirectoryInfo(disc.Name);
                FS_Objects.Add(di);
            }
        }

        return FS_Objects;
    }
    #endregion

    #region Calc
    //static
    public string SizeConvertor(long Bytes) //max line zize  = 7 (123,45K)
    {
        double bytes = Bytes;
        int r = 2;
        if (bytes < 1000)
            return bytes + "B";
        if (bytes > 1000 && bytes < 1000000)
            return Math.Round(bytes / 1000, r) + "K";
        if (bytes > 1000000 && bytes < 1000000000)
            return Math.Round(bytes / 1000000, r) + "M";
        if (bytes > 1000000000 && bytes < 1000000000000)
            return Math.Round(bytes / 1000000000, r) + "G";
        if (bytes > 1000000000000)
            return Math.Round(bytes / 1000000000000, r) + "T";
        return " ";
    }

    public string freeSpace() // 52G/58G (89%)
    {
        //TODO:
        // - use SizeConvertor() (+ this will show other sizes, no only GB )
        // - use SetDiscs(); -> dynamic disk select (at the moment it shows only disk[0] -> "C:\" )
        //SetDiscs();
        //SizeConvertor();
        string name = "";
        const double GB = 1073741824; //bytesToGb

        DriveInfo[] allDrives = DriveInfo.GetDrives();
        var ActiveDrive = allDrives[0];

        double freeSpacePerc = Math.Round((ActiveDrive.AvailableFreeSpace / (float)ActiveDrive.TotalSize) * 100, 0);
        int Total = Convert.ToInt32(ActiveDrive.TotalSize / GB);
        int Free = Convert.ToInt32(ActiveDrive.TotalFreeSpace / GB);

        name = $" {Free}G/{Total}G ({freeSpacePerc}%) ";

        return name;
    }

    // no FileManager specific -> MISC?
    public string Truncated(string ts, int maxLength, string trun = "~")
    {
        if (ts.Length < maxLength)
        {
            //if (Console.BufferWidth > lineLength)
            //{
            //    int pad = (Console.BufferWidth - lineLength);
            //    string space = new String(' ', pad);
            //    return ts + space;
            //}
            return ts;
        }

        maxLength = maxLength - trun.Length;
        int a = maxLength / 2 + maxLength % 2;
        int b = maxLength / 2;
        var truncated = ts.Substring(0, a) + trun + ts.Substring(ts.Length - b, b);

        return truncated;
    }
    #endregion
}
