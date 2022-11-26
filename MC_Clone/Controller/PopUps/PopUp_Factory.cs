using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class PopUp_Factory{}

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
        input = new TextBox() { Label = $"To:", Value = Misc.GetPath(file.FullName, 1) }; //destPath
        components.Add(input);
        Add_BtnOk();
        Add_CancelBtn();
    }


    protected override void BtnOk_Clicked()
    {
        string destPath = input.Value;
        FM.Copy(destPath, file);
        BtnCancel_Clicked();
        Console.Beep();
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
        input = new TextBox() { Label = "To:", Value = Misc.GetPath(file.FullName, 1) };
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
        Console.Beep();
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
        details.Add($"\"{file.Name}\"");
        Add_BtnOk(Config.Error_Accent);
        Add_CancelBtn(Config.Error_Accent);
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
        if (message.Length > 0)
        {
            details.Add(message);
        }
        Button btnDismiss = new Button() { Title = "Dismiss" };
        btnDismiss.asccentColor = Config.PopUp_Accent;
        btnDismiss.Clicked += BtnCancel_Clicked;
        components.Add(btnDismiss);
    }
}

public class GoTo : PopUp
{
    public IntBox input;
    public FileEditor editor;

    private void Start()
    {
        title = "Go To";
        input = new IntBox() { Label = $"Enter row number:", Value = "" };
        components.Add(input);
        Add_BtnOk();
        Add_CancelBtn();
    }
    //public GoTo()
    //{
    //    Start();
    //}
    public GoTo(FileEditor editor)
    {
        Start();
        this.editor = editor;
    }
    protected override void BtnOk_Clicked()
    {
        //Application.window.GetType();
        //Application.PreviewWindow.editor.Offset = value; // goal
        
        editor.Offset = Convert.ToInt32(input.Value); //max 10chars /num (int limit?)
        BtnCancel_Clicked();
    }
}


