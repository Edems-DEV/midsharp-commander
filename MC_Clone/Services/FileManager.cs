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

    public FileManager()
    {
    }

    //public FileManager(string Path)
    //{
    //    path = Path;
    //}

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
}
