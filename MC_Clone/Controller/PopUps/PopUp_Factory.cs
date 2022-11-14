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
}

public class EmptyMsg : PopUp
{
    
}

public class MoveMsg : PopUp
{
    private FileManager FM = new FileManager();
    private TextBox input;
    private FileSystemInfo file;

    public MoveMsg(FileSystemInfo file) //: base()
    {
        this.file = file;

        title = "Move";
        input = new TextBox() { Label = $"Move file {file.Name} - To:", Value = file.FullName };
        components.Add(input);
        Add_BtnOk();
        Add_CancelBtn();
    }


    protected override void BtnOk_Clicked()
    {
        FM.RenMov(input.Value, file);
        BtnCancel_Clicked();
    }
}

public class ErrorMsg : PopUp
{
    public ErrorMsg()
    {
        BackgroundColor = Config.Error_Backgroud;
        ForegroundColor = Config.Error_Foreground;
        AccentColor = Config.Error_Accent;
        
        title = "Error";
        details.Add("something very long --------------------");
        Button btnDismiss = new Button() { Title = "Dismiss" };
        btnDismiss.Clicked += BtnCancel_Clicked;
        components.Add(btnDismiss);
    }
}