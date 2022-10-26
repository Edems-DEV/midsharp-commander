namespace TLab_ConsolePopup;

internal class Program
{
    static void Main(string[] args)
    {
        //PopUp();
        //padCount();
        //FootBar_ControlsPanel();

        int top = Console.WindowHeight;
        int left = Console.WindowWidth;

        PopUp();
        FootBar_ControlsPanel();
        while (true)
        {
            if (top != Console.WindowHeight ||left != Console.WindowWidth) { 
            Console.Clear();
            PopUp();
            FootBar_ControlsPanel();
            top = Console.WindowHeight;
            left = Console.WindowWidth;
            }
        }
    }

    static void PopUp(){
        string text = $"Hello World!";
        CenterLine(text);
        Console.WriteLine(text);
    }

    static void CenterLine(string text)
    {
        int top = Console.WindowHeight;
        int left = Console.WindowWidth;
        Console.SetCursorPosition(left / 2 - (text.Length / 2), top / 2); //center text
    }

    static void padCount() // 60a - 60x
    {
        int top = Console.WindowHeight;
        int left = Console.WindowWidth;
        Console.SetCursorPosition(0, top / 2 + 1);

        for (int i = 0; i < left/2; i++)
        {
            Console.Write("a");
        }
        for (int i = left / 2; i < left; i++)
        {
            Console.Write("x");
        }
    }

    static void FootBar_ControlsPanel()
    {
        int top = Console.WindowHeight;
        int rowLength = Console.WindowWidth;
        Console.SetCursorPosition(0, top - 1); //last visible row
        string[] controls = {"Help", "Menu", "View", "Edit", "Copy", "RenMov", "Mkdir", "Delete", "PullDn", "Quit"};
        int numberPad = 2;

        // 120 - Controls - Count = (var Pad) 
        // bez Controls - pad si to sám vyřeší :)
        int numberSpace = controls.Length * numberPad;
        int totalPad = ((rowLength - numberSpace) / controls.Length);

        int count = 1;
        foreach (var label in controls)
        {
            string number = count.ToString();
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(number.PadLeft(numberPad));

            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(label.PadRight(totalPad));
           
            count++;
        }
        Console.ResetColor();
        Console.WriteLine();
    }
}