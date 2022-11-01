using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class FileManager
{
    private List<FileSystemInfo> FS_Objects = new List<FileSystemInfo>();
    //public FileSystemInfo fileObject = FileSystemInfo(); //FilePanelControll
    //private string _path = ""; //mění se jenom pří ChangeDir
    //nebo static
    
    public FileManager() //can be static?
    {
    }

    #region Import
    public List<FileSystemInfo> SetLists(string path)
    {
        if (this.FS_Objects.Count != 0)
        {
            this.FS_Objects.Clear();
        }
        
        DirectoryInfo levelUpDirectory = null;
        this.FS_Objects.Add(levelUpDirectory);
        //Directories
        string[] directories = Directory.GetDirectories(path);
        foreach (string directory in directories)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            FS_Objects.Add(di);
        }
        //Files
        string[] files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            FileInfo fi = new FileInfo(file);
            FS_Objects.Add(fi);
        }

        return FS_Objects;
    }
    public List<FileSystemInfo> SetDiscs()
    {
        if (FS_Objects.Count != 0)
        {
            FS_Objects.Clear();
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
    #region Fuction
    public string ChangeDir(string path, FileSystemInfo fileObject)
    {
        if (fileObject != null)
        {
            if (fileObject is DirectoryInfo)
            {
                path = fileObject.FullName;
                SetLists(path);
            }
            //else
            //    //file -> F4 edit
        }
        else
        {
            string currentPath = path;
            DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);
            DirectoryInfo upLevelDirectory = currentDirectory.Parent;

            if (upLevelDirectory != null)
            {
                path = upLevelDirectory.FullName;
                SetLists(path);
            }

            else
            {
                SetDiscs();
                return null;
            }
        }
        return path;
    }


    #region Function Keys
    //TODO: delete in FilePanel
    private void CreateFile(string path, string fileName) //Menu
    {
        //if (IsDiscs)
        //    return;
        string destPath = path;
        //string fileName = this.AksName("Enter the file name: "); //TODO: change to popUp
        if (!fileName.Contains('.'))
        {
            fileName = fileName + ".txt";
        }
        try
        {
            string fileFullPath = path + @"\" + fileName;
            DirectoryInfo dir = new DirectoryInfo(fileFullPath);
            if (!File.Exists(fileFullPath))
            {
                using (FileStream fs = File.Create(fileFullPath));
            };
            //else
            //this.ShowMessage("A catalog with that name already exists");
            //SetLists(Path_);
            //UpdatePanel
        }
        catch (Exception e)
        {
            //this.ShowMessage(e.Message);
        }
    }

    private void RenMov(string destPath, FileSystemInfo fileObject)
    {
        //string destPath = this.AksName("Enter the catalog name: "); //TODO: change to popUp
        try
        {
            //FileSystemInfo fileObject = GetActiveObject();
            string objectName = fileObject.Name;
            string destName = Path.Combine(destPath, objectName);
            if (fileObject is FileInfo)
                ((FileInfo)fileObject).MoveTo(destName);
            else
                ((DirectoryInfo)fileObject).MoveTo(destName);
            //SetLists(Path_);
            //UpdatePanel
        }
        catch (Exception e)
        {
            //this.ShowMessage(e.Message);
            return;
        }
    }

    private void CopyDirectory(string sourceDirName, string destDirName) //TODO: change to popUp
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();
        if (!Directory.Exists(destDirName))
            Directory.CreateDirectory(destDirName);
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, true);
        }
        foreach (DirectoryInfo subdir in dirs)
        {
            string temppath = Path.Combine(destDirName, subdir.Name);
            CopyDirectory(subdir.FullName, temppath);
        }
    }

    private void Copy(string destPath, FileSystemInfo fileObject)
    {
        //string destPath = this.AksName("Enter the catalog name: "); //TODO: change to popUp
        try
        {
            //FileSystemInfo fileObject = GetActiveObject();
            FileInfo currentFile = fileObject as FileInfo;
            if (currentFile != null)
            {
                string fileName = currentFile.Name;
                string destName = Path.Combine(destPath, fileName);
                File.Copy(currentFile.FullName, destName, true);
            }
            else
            {
                string currentDir = ((DirectoryInfo)fileObject).FullName;
                string destDir = Path.Combine(destPath, ((DirectoryInfo)fileObject).Name);
                CopyDirectory(currentDir, destDir);
            }
            //SetLists(Path_);
            //UpdatePanel
        }
        catch (Exception e)
        {
            //this.ShowMessage(e.Message);
            return;
        }
    }


    #endregion
    #endregion

    public DriveInfo ActiveDrive(string path)
    {
        //C:\Users\...
        if (path.Length < 3)
            throw new ArgumentException($"Path is too short: {path} - {path.Length} chars");
            
        string activeDisk = path.Substring(0, 3);
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        
        foreach (var drive in allDrives)
        {
            if (drive.Name != activeDisk)
                continue;
            
            return drive;
        }

        return null; //?
    }

    #region Calc
    public long DirSize(DirectoryInfo dir) //static?
    {
        long size = 0;
        // Add file sizes.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            size += file.Length;
        }
        // Add subdirectory sizes.
        DirectoryInfo[] folders = dir.GetDirectories();
        foreach (DirectoryInfo folder in folders)
        {
            size += DirSize(folder);
        }
        return size;
    }
    
    public string FreeSpace(DriveInfo ActiveDrive) // 52G/58G (89%)
    {
        double freeSpacePerc = Math.Round((ActiveDrive.AvailableFreeSpace / (float)ActiveDrive.TotalSize) * 100, 0);
        string total = Truncate.Size(ActiveDrive.TotalSize, 0, false);
        string free = Truncate.Size(ActiveDrive.TotalFreeSpace, 0, false);

        return $" {free}/{total} ({freeSpacePerc}%) ";
    }
    #endregion
}


