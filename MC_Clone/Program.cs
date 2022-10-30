namespace MC_Clone;

internal class Program
{
    static void Main(string[] args)
    {
        //Config.ChangeColorScheme(); //static :)

        Console.CursorVisible = false;
        ConsoleBTN.disable_MaximizeBTN(); //when it is maximized, it only updates after some interaction //TODO: FIX a delete this
        Console.Title = "Midnight Depression";

        Application app = new Application();

        int a = Console.WindowHeight;
        int b = Console.WindowWidth;
        
        while (true)
        {
            Console.BackgroundColor = Config.Primary_BackgroundColor;
            Console.BufferHeight = Console.WindowHeight; //buggy Scroolbar del
            Console.SetCursorPosition(0, 0);
            app.Draw();

            while (!(Console.KeyAvailable))
            {
                //Update if window size was changed
                if (a == Console.WindowHeight && b == Console.WindowWidth) {
                    continue;
                }
                    
                a = Console.WindowHeight; //změni na event
                b = Console.WindowWidth;
                Console.BackgroundColor = Config.Primary_BackgroundColor; //paint whole console
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                app.Draw();
            }
            ConsoleKeyInfo info = Console.ReadKey(); //stuck here //new line (footer problem?)
            app.HandleKey(info);
        }
    }
}