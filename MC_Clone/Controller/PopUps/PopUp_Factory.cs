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
    public FileEditor editor;

    public TextBox input;
    public TextBox input2;

    public Select start;

    public File_Replace(FileEditor editor)
    {
        this.editor = editor;
        title = "Confirm replace";
        input = new TextBox() { Label = $"Enter search string:", Value = "" };
        input2 = new TextBox() { Label = $"Enter replacement string:", Value = "" };
        components.Add(input);
        components.Add(input2);
        Add_BtnOk();
        Add_CancelBtn();
    }
    protected override void BtnOk_Clicked()
    {
        editor.Draw(); //override this popup //broken?
        start = editor.Select.SearchString(input.Value, editor.Cursor.Y_selected);
        Application.SwitchPopUp(new File_ReplaceIn(this));
    }
}
public class File_ReplaceIn : PopUp
{
    File_Replace parent;
    public File_ReplaceIn(File_Replace parent)
    {
        this.parent = parent;
        
        title = "Replace";
        details.Add($"{parent.input.Value}");
        details.Add("Replace with:");
        details.Add($"{parent.input2.Value}");
        Add_BtnOk("Replace");
        Add_ReplaceAllBtn();
        Add_SkipBtn();
        Add_CancelBtn();
    }

    public virtual void BtnCancel_Clicked()
    {
        parent.editor.Select.Selects.Clear();
        base.BtnCancel_Clicked();
    }
    protected override void BtnOk_Clicked()
    {
        //Find + Replace
        parent.editor.Select.ReplaceString(parent.start, parent.input2.Value);
        parent.start = parent.editor.Select.SearchString(parent.input.Value, parent.start.Y);
        parent.editor.Draw();
    }

    #region FindAllBtn
    public void Add_ReplaceAllBtn(string title = "All")
    {
        Button findAll = new Button() { Title = title };
        findAll.Clicked += ReplaceAllBtn_Clicked;
        components.Add(findAll);
    }
    protected void ReplaceAllBtn_Clicked()
    {
        //ReplaceAll
        parent.editor.Select.ReplaceAll(parent.input.Value, parent.input2.Value);
        BtnCancel_Clicked();
    }
    #endregion
    
    #region SkipBtn
    public void Add_SkipBtn(string title = "Skip")
    {
        Button findAll = new Button() { Title = title };
        findAll.Clicked += SkipBtn_Clicked;
        components.Add(findAll);
    }
    protected void SkipBtn_Clicked()
    {
        //Find
        parent.start = parent.editor.Select.SearchString(parent.input.Value, parent.start.Y);
        parent.editor.Draw();
    }
    #endregion
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
        Add_FindAllBtn();
        Add_CancelBtn();
    }
    protected override void BtnOk_Clicked()
    {
        editor.Select.SearchString(input.Value, editor.Cursor.Y_selected);
        BtnCancel_Clicked();
    }

    #region FindAllBtn
    // move to BtnClass?
    public void Add_FindAllBtn(string title = "Find All")
    {
        Button findAll = new Button() { Title = title };
        findAll.Clicked += FindAllBtn_Clicked;
        components.Add(findAll);
    }
    protected void FindAllBtn_Clicked()
    {
        editor.Select.Set_SearchAll(input.Value); //add arguments
        BtnCancel_Clicked();
    }
    #endregion
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


