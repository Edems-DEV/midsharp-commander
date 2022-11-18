using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Application
{
    private Window window; //stack oken
    private PopUp popUp;
    public EventPublisher WinSize = new EventPublisher();

    public string test = "aaaa";

    public Application()
    {
        this.SwitchWindow(new ListWindow(this));
        this.SwitchPopUp(new EmptyMsg()); //change
        //WinSize = new EventPublisher();
    }

    public void HandleKey(ConsoleKeyInfo info)
    {
        if (popUp.GetType() != typeof(EmptyMsg))
        {
            this.popUp.HandleKey(info);
        }
        else
        {
            this.window.HandleKey(info);
        }
    }

    public void Draw()
    {
        

        if (popUp.GetType() != typeof(EmptyMsg))
        {
            this.popUp.Draw();
        }
        else
        {
            this.window.Draw(); //cant be in else -> on resize backgroud is not redrawn
        }
    }

    public void SwitchWindow(Window window)
    {
        window.Application = this; //acces to parent.Object above //redundsnt (window constructor)
        this.window = window; 

        Console.Clear();
    }
    public void SwitchPopUp(PopUp popUp)
    {
        popUp.Application = this; //acces to object above
        this.popUp = popUp;

        //Console.Clear();
    }
}

public class EventPublisher
{
    public event Action OnWindowSizeChange;
    int x;
    int y;

    public EventPublisher()
    {
        x = Console.WindowWidth;
        y = Console.WindowHeight;
    }

    public bool Check()
    {
        if (x != Console.WindowWidth || y != Console.WindowHeight)
        {
            x = Console.WindowWidth;
            y = Console.WindowHeight;
            OnWindowSizeChange?.Invoke();
            return false;
        }
        return true;
    }
}