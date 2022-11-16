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

    public string test = "aaaa";

    public Application()
    {
        this.SwitchWindow(new ListWindow());
        this.SwitchPopUp(new EmptyMsg()); //change
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
        this.window.Draw(); //cant be in else -> on resize backgroud is not redrawn

        if (popUp.GetType() != typeof(EmptyMsg))
        {
            this.popUp.Draw();
        }
    }

    public void SwitchWindow(Window window)
    {
        window.Application = this; //acces to parent.Object above
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
