using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class EditWindow : Window
{
    private List<IComponent> components = new List<IComponent>();
    private int selected = 0;

    private FilesService service;
    int index;

    public EditWindow(int index)
    {
        this.index = index;
        this.service = new FilesService(Config.FILE);

        string[] values = service.Data()[index];

        int i = 0;
        foreach (string item in service.Headers())
        {
            this.components.Add(new TextBox() { Label = item, Value = values[i] });
            i++;
        }


        Button btnOk = new Button() { Title = "OK" };
        btnOk.Clicked += BtnOk_Clicked;

        Button btnCancel = new Button() { Title = "Cancel" };
        btnCancel.Clicked += BtnCancel_Clicked;

        this.components.Add(btnOk);
        this.components.Add(btnCancel);
    }

    private void BtnCancel_Clicked()
    {
        this.Application.SwitchWindow(new ListWindow());
    }

    private void BtnOk_Clicked()
    {
        List<string> data = new List<string>();
        foreach (IComponent item in this.components)
        {
            if (item is TextBox)
            {
                string value = (item as TextBox)!.Value;
                data.Add(value);
            }
        }

        this.service.Update(data.ToArray(), this.index);

        this.Application.SwitchWindow(new ListWindow());
    }

    public override void Draw()
    {
        int i = 0;
        foreach (IComponent item in this.components)
        {
            if (i == this.selected)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            item.Draw();

            Console.ResetColor();
            i++;
        }
    }

    public override void HandleKey(ConsoleKeyInfo info)
    {
        if (info.Key == ConsoleKey.Tab)
        {
            this.selected = (this.selected + 1) % this.components.Count;
        }
        else
        {
            this.components[this.selected].HandleKey(info);
        }
    }
}
