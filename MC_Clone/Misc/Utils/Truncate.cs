using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

internal static class Truncate
{
    public static string Text(string ts, int maxLength, string trun = "~")
    {
        if (ts.Length < maxLength)
        {
            //if (Console.BufferWidth > lineLength)
            //{
            //    int pad = (Console.BufferWidth - lineLength);
            //    string space = new String(' ', pad);
            //    return ts + space;
            //}
            return ts;
        }

        maxLength = maxLength - trun.Length;
        int a = maxLength / 2 + maxLength % 2;
        int b = maxLength / 2;
        var truncated = ts.Substring(0, a) + trun + ts.Substring(ts.Length - b, b);

        return truncated;
    }

    public static string Size(long Bytes)
    {
        //TODO - change to loop
        double bytes = Bytes;
        int r = 2;
        if (bytes < 1000)
            return bytes + "B";
        if (bytes > 1000 && bytes < 1000000)
            return Math.Round(bytes / 1000, r) + "K";
        if (bytes > 1000000 && bytes < 1000000000)
            return Math.Round(bytes / 1000000, r) + "M";
        if (bytes > 1000000000 && bytes < 1000000000000)
            return Math.Round(bytes / 1000000000, r) + "G";
        if (bytes > 1000000000000)
            return Math.Round(bytes / 1000000000000, r) + "T";
        return " ";
    }
}
