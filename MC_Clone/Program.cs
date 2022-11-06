namespace MC_Clone;

internal class Program
{
    static void Main(string[] args)
    {
        //Config.ChangeColorScheme(); //TODO: update config (no more static)

        Console.CursorVisible = false;
        ConsoleBTN.disable_MaximizeBTN(); //when it is maximized, it only updates after some interaction //TODO: FIX a delete this
        Console.Title = "Midnight Depression";

        Application app = new Application();

        int a = Console.WindowHeight;
        int b = Console.WindowWidth;

        int MinHeight = 10;
        int MinWidth = 60;

        while (true)
        {
            Console.BackgroundColor = Config.Primary_BackgroundColor;
            Console.BufferHeight = Console.WindowHeight; //buggy Scroolbar del

            app.Draw();

            while (!(Console.KeyAvailable))
            {

                //Updates if window size was changed
                if (a == Console.WindowHeight && b == Console.WindowWidth)
                {
                    continue;
                }
                if (Console.WindowHeight < MinHeight || Console.WindowWidth < MinWidth)
                {
                    //Console.SetWindowSize(a, b); //broken
                }

                a = Console.WindowHeight;
                b = Console.WindowWidth;
                Misc.ClearConsole();
                app.Draw();
            }
            ConsoleKeyInfo info = Console.ReadKey();
            app.HandleKey(info);
        }
    }
}