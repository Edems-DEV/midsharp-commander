namespace MC_Clone;

internal class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;

        /*
        Table table = new Table(new string[] { "jmeno", "prijmeni", "vek" });

        for (int i = 0; i < 20; i++)
        {
            table.Add(new string[] { "pepa " + i, "novak", i.ToString() });
        }
        */

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