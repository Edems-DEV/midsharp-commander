using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
public class Config
{
    public const string FILE = @"C:\data.txt";
    public const string FOLDER = @"C:\Users\root\Desktop";

    public const ConsoleColor FocusBackgroud = Config.White;
    public const ConsoleColor FocusText = Config.Black;

    public const ConsoleColor ascentColor = Config.DarkCyan;

    public const ConsoleColor Green = ConsoleColor.Green;
    public const ConsoleColor Black = ConsoleColor.Black;
    public const ConsoleColor Gray = ConsoleColor.Gray;

    public const ConsoleColor DarkCyan = ConsoleColor.DarkCyan;
    public const ConsoleColor DarkGray = ConsoleColor.DarkGray;
    public const ConsoleColor White = ConsoleColor.White;
    public const ConsoleColor Red = ConsoleColor.Red; //edit window


    //Primary [Dominant]          -> 
    //Secondary [Complementary]   ->
    //Accent                      -> active, focus

    //Elements
    //ListWindow:
    // - Header: (labels)
    // - FKeys: (Cout + Label)
    // - Table: Text, Focus
    // - inputField: 


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
    public static string Path_LeftPane = @".";
    public static string Path_RightPane = @".";
}