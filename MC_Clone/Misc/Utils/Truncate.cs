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
            return ts;

        maxLength = maxLength - trun.Length;
        int a = maxLength / 2 + maxLength % 2;
        int b = maxLength / 2;
        var truncated = ts.Substring(0, a) + trun + ts.Substring(ts.Length - b, b);

        return truncated;
    }

    public static string Size(double bytes, int round = 0, bool space = true)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        while (bytes >= 1024 && order < sizes.Length - 1)
        {
            order++;
            bytes = bytes / 1024;
        }
        string result = "";
        result += Math.Round(bytes, round).ToString();
        if (space)
            result += " ";
        result += sizes[order];

        return result;
    }
}
