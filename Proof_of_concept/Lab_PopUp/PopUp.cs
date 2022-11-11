using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Lab_PopUp;
internal class PopUp : IComponent
{
    int x_center = 0;
    int y_center = 0; // = start y

    int height = 2 + 8 + 8;
    int width;

    int X_DeadSpace = 5;

    string title = "Title";
    string info = "Do you want to save changes?";

    public List<Button> buttons = new List<Button>();
    public List<TextBox> textBoxes= new List<TextBox>();
    public List<IComponent> components = new List<IComponent>();
        
    public PopUp( string title, string info, int width = 50)
    {
        x_center = (Console.WindowWidth  / 2);
        y_center = (Console.WindowHeight / 3);

        this.width = width;
        this.title = title;
        this.info = info;
    }

    public void Draw()
    {
        int c = 1; //y counter
        y_center += DrawBox(); //first -> backgroud color
        DrawTitle(c++); //y += -> píše se v drawbox
        y_center += DrawInfo(c++);
        if (textBoxes.Count > 0)
        {
            y_center += Line('├', '┤', '─', c++);
            y_center += DrawTextBoxes(c);  
        }
        
        y_center += Line('├', '┤', '─', c++);
        y_center += DrawButtons(c++);
        height = c + 2;
    }

    public void Draw(string title, string info, int width = 50)
    {
        this.title = title;
        this.info = info;
        Draw();
    }

    public void HandleKey(ConsoleKeyInfo info)
    { 
    }

    public int DrawBox()
    {
        int yy = 0;
        int ySize;


        yy += Line(' ', ' ', ' ', yy);
        yy += Line('┌', '┐', '─', yy);
        ySize = yy;
        
        while (yy < height - ySize)
        {
            yy += Line('│', '│', ' ', yy);
        }
        yy += Line('└', '┘', '─', yy);
        yy += Line(' ', ' ', ' ', yy);

        return ySize;
    }

    public void DrawTitle(int yy)
    {
        ConsoleColor oldTextC = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.BackgroundColor = ConsoleColor.Gray;

        title = $" {title} "; //spaces

        try { 
        Console.SetCursorPosition(XCenter(title), y_center - 1);
        }
        catch { throw new Exception($"{XCenter(title)}"); }
        Console.Write(title);

        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;
    }
    public int DrawInfo(int yy)
    {
        ConsoleColor oldTextC = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Black;
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.BackgroundColor = ConsoleColor.Gray;

        Console.SetCursorPosition(XCenter(info), y_center);
        if (info.Length > width - 4)
        {
            throw new ArgumentException($"Text overflow box - text.l = {info.Length} | box.w = {width}");
        }
        Console.Write(info);
        int ySize = 1;

        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;

        return ySize;
    }

    public int Line(char start = '└', char end = '┘', char fill = '─', int yy = 0)
    {
        ConsoleColor oldBackC = Console.BackgroundColor;
        ConsoleColor oldTextC = Console.ForegroundColor;
        Console.BackgroundColor = ConsoleColor.Gray;
        Console.ForegroundColor = ConsoleColor.Black;

        
        string buf = "";

        buf += " ";
        buf += start;
        for (int i = 0; i <= width - X_DeadSpace; i++) //-1 -> '<='
        {
            buf += fill;
        }
        buf += end;
        buf += " ";
        Console.SetCursorPosition(XCenter(buf), y_center + yy);
        Console.Write(buf);

        
        Console.BackgroundColor = oldBackC;
        Console.ForegroundColor = oldTextC;

        int ySize = 1;
        return ySize;
    }

    public int DrawTextBoxes(int yy)
    {
        ConsoleColor oldTextC = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Black;
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.BackgroundColor = ConsoleColor.Blue;

        int ySize = 0;

        foreach (var textbox in textBoxes)
        {
            textbox.Size = width - X_DeadSpace;
            ySize = textbox.Draw(XCenter(width) + 2, y_center +1);
        }

        return ySize;
    }

    public int DrawButtons(int yy)
    {
        ConsoleColor oldTextC = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Black;
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.BackgroundColor = ConsoleColor.Gray;

        int TotalWidth = 0;
        int space = 2;
        //4 = button border
        foreach (var button in buttons)
        {
            TotalWidth += button.Title.Length + 4;
            TotalWidth += space;
        }
        TotalWidth -= 5; //idk - just works to center it

        Console.SetCursorPosition(XCenter(TotalWidth), y_center + yy);
        foreach (var button in buttons)
        {
            button.Draw();
            Console.Write(" ");
        }

        Console.SetCursorPosition(0, 0); //debug console
        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;

        int ySize = 1;
        return ySize;
    }

    public void Clear()
    {
        Console.Clear();
        x_center = (Console.WindowWidth / 2);
        y_center = (Console.WindowHeight / 3);
    }

    public int XCenter(string text)
    {
        return (x_center - (text.Length / 2));
    }
    public int XCenter(int text)
    {
        return (x_center - (text / 2));
    }

    #region Old
    static void PopUp_OLD()
    {
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
    #endregion
}
