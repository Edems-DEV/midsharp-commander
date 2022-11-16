using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

internal static class Truncate
{
    public static string Path(string ts, int maxLength = 40, string trun = @"~\") //TODO: finish it
    {
        if (ts.Length < maxLength)
            return ts;
        
        List<string> PathNodes = ts.Split('\\').ToList();

        string leftTruncated = "";
        string disk = PathNodes[0] + @"\";
        PathNodes.RemoveAt(0); //disk
        PathNodes.RemoveAt(PathNodes.Count - 1); //file

        string rightTruncated = "";

        PathNodes.Reverse();
        for (int i = PathNodes.Count - 1; i >= 0; i--)
        {
            if (i == 3) { leftTruncated = disk; continue; }
            if (maxLength > rightTruncated.Length + leftTruncated.Length + PathNodes[i].Length + trun.Length)
                rightTruncated += PathNodes[i] + @"\";
            else { break; }
        }
        
        string truncated = leftTruncated + trun + rightTruncated;
        return truncated;
    }
    
    public static string Text(string ts, int maxLength, string trun = "~")
    {
        if (ts.Length < maxLength)
            return ts;

        maxLength = maxLength - trun.Length - 2; //-5; //why?
        int a = maxLength / 2 + maxLength % 2;
        int b = maxLength / 2;
        if (a < 0)
            return trun;

        var truncated = ts.Substring(0, a) + trun;
        if (ts.Length - b > 0)
            truncated += ts.Substring(ts.Length - b, b);

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
