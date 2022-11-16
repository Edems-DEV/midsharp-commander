using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class PopUp_Factory
{
    static public void Error_Dismiss()
    {
        //PopUp p = new PopUp("Close file", "\"/Home/root/\" is not a regular file");
        //p.components.Add(new Button() { Title = "Dismiss" });
        //p.BackgroundColor = Config.Error_Backgroud;
        //p.ForegroundColor = Config.Error_Foreground;
        //p.AccentColor = Config.Error_Accent;
        //return p;
    }

    static public void Error_Delete()
    {
        List<string> Lines = new List<string>();
        Lines.Add("Delete file");
        Lines.Add("\"ChangeMe.txt\"?");
        //PopUp p = new PopUp("Delete", Lines);
        //p.components.Add(new Button() { Title = "Yes" });
        //p.components.Add(new Button() { Title = "no" });
        //p.BackgroundColor = Config.Error_Backgroud;
        //p.ForegroundColor = Config.Error_Foreground;
        //p.AccentColor = Config.Error_Accent;
        //return p;
    }

    static public void Move()
    {
        //PopUp p = new PopUp("Move", "File: {filename}"); //optimize for empty details
        //p.components.Add(new TextBox() { Label = "Destination Path:", Value = @"C:\Users\root\Desktop\Example" }); //TODO: path truncate
        //p.components.Add(new Button() { Title = "Ok" });
        //p.components.Add(new Button() { Title = "Cancel" });

        //return p;
    }

    //private static void SetError()
    //{
    //    BackgroundColor = Config.Error_Backgroud;
    //    ForegroundColor = Config.Error_Foreground;
    //    AccentColor = Config.Error_Accent;
    //}
}

public class EmptyMsg : PopUp
{
    
}
public class CreateFileMsg : PopUp
{
    private FileManager FM = new FileManager();
    private TextBox input;
    string path;

    public CreateFileMsg(string path)
    {
        this.path = path;

        title = "Create a new File";
        input = new TextBox() { Label = $"Enter file name:", Value = "" };
        components.Add(input);
        Add_BtnOk();
        Add_CancelBtn();
    }


    protected override void BtnOk_Clicked()
    {
        if (!input.Value.Contains("."))
            input.Value += ".txt";
        
        FM.MkFile(path, input.Value);
        BtnCancel_Clicked();
    }
}
public class CreateFolderMsg : PopUp
{
    private FileManager FM = new FileManager();
    private TextBox input;
    string path;

    public CreateFolderMsg(string path)
    {
        this.path = path;

        title = "Create a new Folder";
        input = new TextBox() { Label = $"Enter folder name:", Value = "" };
        components.Add(input);
        Add_BtnOk();
        Add_CancelBtn();
    }


    protected override void BtnOk_Clicked()
    {
        FM.CreateFolder(path, input.Value);
        BtnCancel_Clicked();
    }
}
public class CopyMsg : PopUp
{
    private FileManager FM = new FileManager();
    private TextBox input;
    private FileSystemInfo file;

    public CopyMsg(FileSystemInfo file) //: base()
    {
        this.file = file;

        title = "Copy";
        details.Add($"Copy: \"{file.Name}\"");
        //details.Add($"from: \"{Truncate.Path(file.FullName)}\"");
        input = new TextBox() { Label = $"To:", Value = Misc.GetPath(file.Name) }; //destPath
        components.Add(input);
        Add_BtnOk();
        Add_CancelBtn();
    }


    protected override void BtnOk_Clicked()
    {
        string destPath = input.Value;
        FM.Copy(destPath, file);
        BtnCancel_Clicked();
    }
}

public class MoveMsg : PopUp
{
    private FileManager FM = new FileManager();
    private TextBox input;
    private TextBox input2;
    private FileSystemInfo file;

    public MoveMsg(FileSystemInfo file)
    {
        this.file = file;

        title = "Move";
        details.Add($"Move: \"{file.Name}\"");
        input = new TextBox() { Label = "To:", Value = Misc.GetPath(file.Name) };
        input2 = new TextBox() { Label = "Rename to:", Value = file.Name };
        components.Add(input);
        components.Add(input2);
        Add_BtnOk();
        Add_CancelBtn();
    }


    protected override void BtnOk_Clicked()
    {

        FM.RenMov(input.Value, file, input2.Value);
        BtnCancel_Clicked();
    }
}

public class DeleteMsg : PopUp_Red
{
    private FileManager FM = new FileManager();
    private FileSystemInfo file;

    public DeleteMsg(FileSystemInfo file)
    {
        this.file = file;

        title = "Delete";
        Add_BtnOk();
        Add_CancelBtn();
    }

    protected override void BtnOk_Clicked()
    {
        FM.Delete(file);
        BtnCancel_Clicked();
    }
}

public class ErrorMsg : PopUp_Red
{
    public ErrorMsg(string message)
    {   
        title = "Error";
        details.Add(message);
        Button btnDismiss = new Button() { Title = "Dismiss" };
        btnDismiss.Clicked += BtnCancel_Clicked;
        components.Add(btnDismiss);
    }
}


