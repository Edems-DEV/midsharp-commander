namespace Example;

internal class Program
{
    static void Main(string[] args)
    {
        App app = new App();
    }
}

public class Config
{
    public string Color { get; set; } = "You can access 'Config.Color'";
}

public class App
{
    private Window window; //stack oken
    public Config config;

    public App()
    {
        config = new Config();
        SwitchWindow(new MainWin(this));
        window.Draw();
        Console.WriteLine("-----------------");
        window.Draw();
    }

    public void SwitchWindow(Window window)
    {
        window.Application = this; //acces to parent.Object above / but after Constructor
        this.window = window;
    }
    public void ChangeTheme()
    {
        config.Color = "New color scheme";
    }
}

public abstract class Window
{
    public App Application { get; set; }
    public abstract void Draw();
}

public class MainWin : Window
{
    SubWin pane;
    public MainWin()
    {
        pane = new SubWin(Application);
    }
    public MainWin(App application)
    {
        Application = application;
        pane = new SubWin(Application);
    }

    public override void Draw()
    {
        Console.WriteLine("1) MainWin: " + this.Application.config.Color);
        pane.Draw();
    }
}

public class SubWin : Window
{
    public App Application { get; set; }
    
    public SubWin(App Application)
    {
        this.Application = Application;
    }
    public override void Draw()
    {
        Console.WriteLine("2) SubWin: " + this.Application.config.Color);
        ChangeTheme();
    }
    private void ChangeTheme() //user input
    {
        Application.ChangeTheme();
    }
}

