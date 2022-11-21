using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

public interface IComponent
{
    public void HandleKey(ConsoleKeyInfo info);

    public void Draw();
}
