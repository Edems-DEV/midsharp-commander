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

        bool sizeChanged;

        while (true)
        {
            Console.BackgroundColor = Config.Primary_BackgroundColor;
            Console.BufferHeight = Console.WindowHeight; //buggy Scroolbar del

            app.Draw();

            while (!(Console.KeyAvailable))
            {
                sizeChanged = app.WinSize.CheckIfSizeChanged(); //before 

                //Updates if window size was changed
                if (sizeChanged)
                {
                    continue;
                }
                //if (Console.WindowHeight < MinHeight || Console.WindowWidth < MinWidth)
                //{
                //    //Console.SetWindowSize(a, b); //broken
                //}

                //Misc.ClearConsole();
                app.Draw();
            }
            ConsoleKeyInfo info = Console.ReadKey();
            app.HandleKey(info);
        }
    }
}

