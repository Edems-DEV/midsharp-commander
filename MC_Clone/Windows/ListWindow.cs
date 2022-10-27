using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public class ListWindow : Window
{
    private Table table;

    public ListWindow()
    {
        FilesService service = new FilesService(Config.FILE);

        this.table = new Table(service.Headers());

        foreach (string[] item in service.Data())
        {
            table.Add(item);
        }

        this.table.RowSelected += Table_RowSelected;
    }

    private void Table_RowSelected(int index)
    {
        this.Application.SwitchWindow(new EditWindow(index));
    }

    public override void Draw()
    {
        this.table.Draw();
    }

    public override void HandleKey(ConsoleKeyInfo info)
    {
        this.table.HandleKey(info);
    }
}
