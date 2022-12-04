using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Select
{
    //start positions
    public int X;
    public int Y;

    public string Content; //prep for redex

    public Select(int x, int y, string content) 
    {
        X = x;
        Y = y;
        Content = content;
    }
}
