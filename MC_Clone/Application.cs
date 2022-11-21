using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Application
{
    public Window window; //stack oken
    public PopUp popUp;// = new EmptyMsg();
    public EventPublisher WinSize = new EventPublisher();
    bool a = true;

    public Application()
    {
        this.SwitchWindow(new ListWindow(this));
        this.SwitchPopUp(new EmptyMsg());
        WinSize.Application = this;

        //WinSize.OnWindowSizeChange += window.Draw; //on EditWindow still draw ListWindow
        WinSize.OnWindowSizeChange += popUp.Draw; //use SwitchPopUp obeject (why?)
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

    int MsgStartSize = 0;

    public Application Application { get; set; }

    public EventPublisher()
    {
        x = Console.WindowWidth;
        y = Console.WindowHeight;
    }

    public bool CheckIfSizeChanged()
    {
        CheckMsgSize();
        return CheckWinSize();
    }
    public bool CheckWinSize()
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

    public void CheckMsgSize()
    {
        if (Application.popUp.GetType() == typeof(EmptyMsg)) { return; }

        if (Application.popUp.width < MsgStartSize)
        {
            OnWindowSizeChange?.Invoke(); // -> it switch to empty msg (WHY????)
            //default size -> ok
            //one char larger + backspace -> empty msg
            //right size -> ok again
            //problem -> use SwitchPopUp obeject on backspace (why?)
        }

        MsgStartSize = Application.popUp.width;
    }
}