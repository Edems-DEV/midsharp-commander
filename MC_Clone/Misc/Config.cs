using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Config
{
    
    public const string FOLDER = @"C:\Users\root\Desktop";
    public const string Path_LeftPane = @"C:\Users\root\Desktop";
    public const string Path_RightPane = @".";

    // Color scheme
    public static ConsoleColor Primary_BackgroundColor = ConsoleColor.DarkBlue;
    public static ConsoleColor Primary_ForegroundColor = ConsoleColor.White;
    public static ConsoleColor Secondary_BackgroundColor = ConsoleColor.DarkCyan;
    public static ConsoleColor Secondary_ForegroundColor = ConsoleColor.Black;
    public static ConsoleColor Accent_BackgroundColor = ConsoleColor.Gray;
    public static ConsoleColor Accent_ForegroundColor = ConsoleColor.Black;



    //Elements specific
    //Focus
    public static ConsoleColor Table_Path_BackgroundColor = Primary_BackgroundColor;
    public static ConsoleColor Table_Path_ForegroundColor = Primary_ForegroundColor;
    public static ConsoleColor Table_Path_ACTIVE_BackgroundColor = Accent_BackgroundColor;
    public static ConsoleColor Table_Path_ACTIVE_ForegroundColor = Accent_ForegroundColor;
    public static ConsoleColor Table_Line_BackgroundColor = Secondary_BackgroundColor;
    public static ConsoleColor Table_Line_ForegroundColor = Secondary_ForegroundColor;
    public static ConsoleColor Table_Line_ACTIVE_BackgroundColor = Secondary_BackgroundColor;
    public static ConsoleColor Table_Line_ACTIVE_ForegroundColor = Secondary_ForegroundColor;
    //Other
    public static ConsoleColor Labels_BackgroundColor = Secondary_BackgroundColor;
    public static ConsoleColor Labels_ForegroundColor = Secondary_ForegroundColor;
    public static ConsoleColor Cout_BackgroundColor = ConsoleColor.Black;
    public static ConsoleColor Cout_ForegroundColor = ConsoleColor.White;

    public static ConsoleColor Table_BackgroundColor = Primary_BackgroundColor;
    public static ConsoleColor Table_ForegroundColor = Primary_ForegroundColor;


    public static ConsoleColor MsgBoxBackgroundColor           = ConsoleColor.White;
    public static ConsoleColor MsgBoxForegroundColor           = ConsoleColor.Black;


    public static ConsoleColor AdditionalMsgBoxBackgroundColor = ConsoleColor.Red;
    public static ConsoleColor TextBoxBackgroundColor          = ConsoleColor.Gray;


    //Legacy (TODO: delete)
    public const string FILE = @"C:\data.txt";

    #region Notes
    //Primary [Dominant]          -> 
    //Secondary [Complementary]   ->
    //Accent                      -> active, focus

    //Elements
    //ListWindow:
    // - Header: (labels)
    // - FKeys: (Cout + Label)
    // - Table: Text, Focus
    // - inputField: 
    #endregion
}