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
    int x = Console.WindowWidth / 2;
    int y = (Console.WindowHeight / 3);

    int height = 2 + 8;
    int width = 50;

    public List<Button> buttons = new List<Button>();
    public List<TextBox> textBoxes= new List<TextBox>();
    public List<IComponent> components = new List<IComponent>();
        


    public void Draw()
    {
        int c = 1; //y counter
        Box(); //first -> backgroud color
        Title("|", c++);
        Info("Do you want to save changes?", c++);
        if (textBoxes.Count > 0)
        {
            Line('├', '┤', '─', c++);
            c += DrawTextBoxes(c);  
        }
        
        Line('├', '┤', '─', c++);
        DrawButtons(c++);
        height = c + 2;
    }
    
    

    public void HandleKey(ConsoleKeyInfo info)
    { 
    }


    
    
    public void Box()
    {
        int yy = 0;
        
        yy += Line(' ', ' ', ' ', yy);
        yy += Line('┌', '┐', '─', yy);
        
        while (yy < height - 2)
        {
            yy += Line('│', '│', ' ', yy);
        }
        yy += Line('└', '┘', '─', yy);
        yy += Line(' ', ' ', ' ', yy);
    }

    public void Title(string label, int yy)
    {
        ConsoleColor oldTextC = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.BackgroundColor = ConsoleColor.Gray;

        label = $" {label} "; //spaces

        Console.SetCursorPosition(x - (label.Length / 2), y + yy);
        Console.Write(label);

        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;
    }
    public void Info(string text, int yy)
    {
        ConsoleColor oldTextC = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Black;
        ConsoleColor oldBackC = Console.BackgroundColor;
        Console.BackgroundColor = ConsoleColor.Gray;

        Console.SetCursorPosition(x - (text.Length / 2), y + yy);
        if (text.Length > width - 4)
        {
            throw new ArgumentException($"Text overflow box - text.l = {text.Length} | box.w = {width}");
        }
        Console.Write(text);

        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;
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
        for (int i = 0; i < width - 2 - 2; i++)
        {
            buf += fill;
        }
        buf += end;
        buf += " ";
        Console.SetCursorPosition(x - (buf.Length/2), y + yy);
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
            ySize = textbox.Draw(x - (width / 2) + 2, y + yy +1);
        }

        return ySize;
    }

    public void DrawButtons(int yy)
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

        Console.SetCursorPosition(x - (TotalWidth / 2), y + yy);
        foreach (var button in buttons)
        {
            button.Draw();
            Console.Write(" ");
        }

        Console.SetCursorPosition(0, 0); //debug console
        Console.ForegroundColor = oldTextC;
        Console.BackgroundColor = oldBackC;
    }
}
