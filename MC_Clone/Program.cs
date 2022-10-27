namespace MC_Clone;

internal class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;

        Application app = new Application();

        while (true)
        {
            Console.SetCursorPosition(0, 0);
            app.Draw();

            ConsoleKeyInfo info = Console.ReadKey();
            app.HandleKey(info);
        }
    }
}