namespace Lab_PopUp;

internal class Program
{
    static void Main(string[] args)
    {
        //YesNoMessageBox y = new YesNoMessageBox("Hello", "World");
        //y.GetMessageBox();
        int size = 20;
        PopUp p = new PopUp("Joe", "no", 50);
        p.buttons.Add(new Button() { Title = "Yes" });
        p.buttons.Add(new Button() { Title = "Yes" });
        p.buttons.Add(new Button() { Title = "Yes" });
        p.textBoxes.Add(new TextBox() { Label = "Name", Value = "John" });
        p.textBoxes.Add(new TextBox() { Label = "Path", Value = @"C:\Users\" });
        p.Draw();
    }
}
