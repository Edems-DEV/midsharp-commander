namespace Lab_PopUp;

internal class Program
{
    static void Main(string[] args)
    {
        //PROBLEM: same object -> values wont reset -> broken UI

        Console.CursorVisible = false;

        int a = Console.WindowHeight;
        int b = Console.WindowWidth;

        var p = Move();
        p.Draw();

        while (true) {
            //Updates if window size was changed
            if (a == Console.WindowHeight && b == Console.WindowWidth)
            {
                continue;
                //Thread.Sleep(1000); //debug
            }
            Console.BackgroundColor = Config.BackgroundColor;
            a = Console.WindowHeight;
            b = Console.WindowWidth;
            p.Clear();
            p.Draw();
        }
    }

    static public PopUp aaaa()
    {
        List<string> Lines = new List<string>();
        Lines.Add("File /home/text.txt");
        Lines.Add("Save before close?");
        PopUp p = new PopUp("Close file", Lines);
        p.buttons.Add(new Button() { Title = "Yes" });
        p.buttons.Add(new Button() { Title = "Yes" });
        p.buttons.Add(new Button() { Title = "Yes" });
        p.textBoxes.Add(new TextBox() { Label = "Name", Value = @"C:\Users\" });
        p.textBoxes.Add(new TextBox() { Label = "Path", Value = @"C:\Users\" });
        return p;
    }
    
    static public PopUp Error_Dismiss()
    {
        PopUp p = new PopUp("Close file", "\"/Home/root/\" is not a regular file");
        p.buttons.Add(new Button() { Title = "Dismiss" });
        p.BackgroundColor = ConsoleColor.Red;
        p.ForegroundColor = ConsoleColor.White;
        p.AccentColor = ConsoleColor.Yellow;
        return p;
    }

    static public PopUp Error_Delete()
    {
        List<string> Lines = new List<string>();
        Lines.Add("Delete file");
        Lines.Add("\"ChangeMe.txt\"?");
        PopUp p = new PopUp("Delete", Lines);
        p.buttons.Add(new Button() { Title = "Yes" });
        p.buttons.Add(new Button() { Title = "no" });
        p.BackgroundColor = ConsoleColor.Red;
        p.ForegroundColor = ConsoleColor.White;
        p.AccentColor = ConsoleColor.Yellow;
        return p;
    }
    
    static public PopUp Move()
    {
        PopUp p = new PopUp("Move", "File: {filename}"); //optimize for empty details
        p.textBoxes.Add(new TextBox() { Label = "Destination Path:", Value = @"C:\Users\root\Desktop\Example" }); //TODO: path truncate
        p.buttons.Add(new Button() { Title = "Ok" });
        p.buttons.Add(new Button() { Title = "Cancel" });
        return p;
    }
}

public static class PopUpFactory
{

}
