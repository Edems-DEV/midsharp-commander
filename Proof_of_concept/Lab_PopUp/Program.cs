namespace Lab_PopUp;

internal class Program
{
    static void Main(string[] args)
    {
        //YesNoMessageBox y = new YesNoMessageBox("Hello", "World");
        //y.GetMessageBox();
        PopUp p = new PopUp();
        p.buttons.Add(new Button() { Title = "Yes" });
        p.buttons.Add(new Button() { Title = "Yes" });
        p.buttons.Add(new Button() { Title = "Yes" });
        p.textBoxes.Add(new TextBox() { Label = "Name", Value = "John" });
        p.Draw();
    }
}
