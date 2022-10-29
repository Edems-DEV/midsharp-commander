namespace MC_Clone;

internal class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;

        Application app = new Application();

        int a = Console.WindowHeight;
        int b = Console.WindowWidth;
        
        while (true)
        {
            Console.SetCursorPosition(0, 0);
            app.Draw();

            while (!(Console.KeyAvailable))
            {
                //System.Threading.Thread.Sleep(50);
                if (a == Console.WindowHeight && b == Console.WindowWidth) { //window size is same
                    //System.Threading.Thread.Sleep(1000);
                    //Console.WriteLine(a + " / " + Console.BufferHeight + "||" + b + " / " + Console.BufferWidth);
                    continue;
                }
                    
                a = Console.WindowHeight; //změni na event
                b = Console.WindowWidth;
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                app.Draw();
            }
            ConsoleKeyInfo info = Console.ReadKey(); //stuck here //new line (footer problem?)
            app.HandleKey(info);
        }
    }
}