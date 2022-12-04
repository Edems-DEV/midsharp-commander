using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class PopUp_Factory{} //rename to PopUp_Builder

public class EmptyMsg : PopUp
{
    
}

public class NotFound : PopUp
{
    public NotFound(string title, string details)
    {
        this.title = title;
        this.details.Add(details);
        Add_CancelBtn("Dismiss");
    }
}

public class File_Replace : PopUp
{
    private TextBox input;
    private TextBox input2;

    public File_Replace()
    {
        title = "Replace";
        input = new TextBox() { Label = $"Enter search string:", Value = "" };
        input2 = new TextBox() { Label = $"Enter replacement string:", Value = "" };
        components.Add(input);
        components.Add(input2);
        Add_BtnOk();
        Add_CancelBtn();
    }
    protected override void BtnOk_Clicked()
    {
    }
}

public class File_Search : PopUp
{
    private TextBox input;
    private FileEditor editor;

    public File_Search(FileEditor editor)
    {
        this.editor = editor;

        title = "Search";
        input = new TextBox() { Label = $"Enter search string:", Value = "" };
        components.Add(input);
        Add_BtnOk();
        Add_CancelBtn();
    }
    protected override void BtnOk_Clicked()
    {
        editor.Select.SearchString(input.Value, editor.Cursor.Y_selected);
        BtnCancel_Clicked();
    }
}
public class CloseFile : PopUp
{
    private MyFileService FS;
    List<string> Rows;

    public CloseFile(FileSystemInfo file, List<string> rows)
    {
        Rows = rows;
        FS = new MyFileService(file.FullName);
        title = "Close file";
        details.Add($"File: \"{file.Name}\" was modified.");
        details.Add($"Save before close?");
        Add_BtnOk();
        Add_NoBtn();
        Add_CancelBtn();
    }


    protected override void BtnOk_Clicked()
    {
        FS.OverWrite(Rows);
        BtnNo_Clicked();
    }
    #region NoBtn
    // move to BtnClass?
    public void Add_NoBtn(string title = "No")
    {
        Button btnNo = new Button() { Title = title };
        btnNo.Clicked += BtnNo_Clicked;
        components.Add(btnNo);
    }
    protected void BtnNo_Clicked()
    {
        BtnCancel_Clicked();
        Application.SwitchWindow(new ListWindow(Application));
    }
    #endregion
}
public class SaveFile : PopUp
{
    private MyFileService FS;
    List<string> Rows;

    public SaveFile(FileSystemInfo file, List<string> rows)
    {
        Rows = rows;
        FS = new MyFileService(file.FullName);
        title = "Save file";
        details.Add($"Confirm save file: \"{file.Name}\"");
        Add_BtnOk();
        Add_CancelBtn();
    }


    protected override void BtnOk_Clicked()
    {
        FS.OverWrite(Rows);
        BtnCancel_Clicked();
    }
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
        Add_CancelBtn("No");
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
    public FilePreview editor;

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
    public GoTo(FilePreview editor)
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


