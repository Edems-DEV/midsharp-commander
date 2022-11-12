using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;

internal static class l
{
    public static char topLeft = '┌';
    public static char topRight = '┐';
    public static char bottomLeft = '└';
    public static char bottomRight = '┘';
    public static char lineX = '─';
    public static char lineY = '│';
    public static char upRight = '├';
    public static char upleft = '┤';
    public static char down = '┬';
    public static char cross = '┼';
    public static char up = '┴';
    public static char arrowRight = '<';

    //public static char topLeft = '╔';
    //public static char topRight = '╗';
    //public static char bottomLeft = '╚';
    //public static char bottomRight = '╝';
    //public static char lineX = '═';
    //public static char lineY = '║';
    //public static char upRight = '╠';
    //public static char upleft = '╣';
    //public static char down = '╦';
    //public static char cross = '╬';
    //public static char up = '╩';
    //public static char arrowRight = '◄';

}

public class Config
{
    public const string Path_LeftPane = @"C:\Users\root\Desktop"; //change to dynamic desktop path
    public const string Path_RightPane = @".";

    //------------------Color schemes------------------
    public static ConsoleColor[] CScheme_Legacy = { ConsoleColor.Black    , ConsoleColor.White ,
                                                    ConsoleColor.DarkCyan , ConsoleColor.Black ,
                                                    ConsoleColor.Gray     , ConsoleColor.Black};

    public static ConsoleColor[] CScheme_Original = { ConsoleColor.DarkBlue , ConsoleColor.White ,
                                                      ConsoleColor.DarkCyan , ConsoleColor.Black ,
                                                      ConsoleColor.Gray     , ConsoleColor.Black};
    
    public static ConsoleColor[] CScheme_White = { ConsoleColor.White   , ConsoleColor.Black,
                                                   ConsoleColor.DarkGray, ConsoleColor.Gray,
                                                   ConsoleColor.Black   , ConsoleColor.DarkGray };

    public static ConsoleColor[] CSchemesMaster = CScheme_Original;

    //------------------Color scheme------------------
    public static ConsoleColor Primary_BackgroundColor = CSchemesMaster[0];
    public static ConsoleColor Primary_ForegroundColor = CSchemesMaster[1];
    public static ConsoleColor Secondary_BackgroundColor = CSchemesMaster[2];
    public static ConsoleColor Secondary_ForegroundColor = CSchemesMaster[3];
    public static ConsoleColor Accent_BackgroundColor = CSchemesMaster[4];
    public static ConsoleColor Accent_ForegroundColor = CSchemesMaster[5];



    //------------------Elements specific------------------
    //ListWindow - Elements
    public static ConsoleColor Table_BackgroundColor             = Primary_BackgroundColor;
    public static ConsoleColor Table_ForegroundColor             = Primary_ForegroundColor;
    public static ConsoleColor Table_Line_BackgroundColor        = Secondary_BackgroundColor;
    public static ConsoleColor Table_Line_ForegroundColor        = Secondary_ForegroundColor;
    public static ConsoleColor Table_Line_ACTIVE_BackgroundColor = Secondary_BackgroundColor;
    public static ConsoleColor Table_Line_ACTIVE_ForegroundColor = Secondary_ForegroundColor;
    public static ConsoleColor Table_Path_BackgroundColor        = Primary_BackgroundColor;
    public static ConsoleColor Table_Path_ForegroundColor        = Primary_ForegroundColor;
    public static ConsoleColor Table_Path_ACTIVE_BackgroundColor = Accent_BackgroundColor;
    public static ConsoleColor Table_Path_ACTIVE_ForegroundColor = Accent_ForegroundColor;

    public static ConsoleColor Labels_BackgroundColor = Secondary_BackgroundColor;
    public static ConsoleColor Labels_ForegroundColor = Secondary_ForegroundColor;
    public static ConsoleColor Cout_BackgroundColor   = ConsoleColor.Black;
    public static ConsoleColor Cout_ForegroundColor   = ConsoleColor.White;

    public static ConsoleColor MsgBoxBackgroundColor           = ConsoleColor.White;
    public static ConsoleColor MsgBoxForegroundColor           = ConsoleColor.Black;


    public static ConsoleColor AdditionalMsgBoxBackgroundColor = ConsoleColor.Red;
    public static ConsoleColor TextBoxBackgroundColor          = ConsoleColor.Gray;

    public static ConsoleColor PopUp_Backgroud = ConsoleColor.Gray;
    public static ConsoleColor PopUp_ForeGroud = ConsoleColor.Black;
    public static ConsoleColor PopUp_Accent    = ConsoleColor.DarkBlue;

    public static ConsoleColor Error_Backgroud  = ConsoleColor.Red;
    public static ConsoleColor Error_Foreground = ConsoleColor.White;
    public static ConsoleColor Error_Accent     = ConsoleColor.DarkYellow;




    public static void ChangeColorScheme(ConsoleColor[] colors) ////static cannot be changed ? 
    {
        Primary_BackgroundColor = ConsoleColor.Black; //colors[0];
        Primary_ForegroundColor = colors[1];
        Secondary_BackgroundColor = colors[2];
        Secondary_ForegroundColor = colors[3];
        Accent_BackgroundColor = colors[4];
        Accent_ForegroundColor = colors[5];
    }

    public static void ChangeColorScheme() //debug
    {
        Primary_BackgroundColor = ConsoleColor.Magenta;
        Primary_ForegroundColor = ConsoleColor.Magenta;
        Secondary_BackgroundColor = ConsoleColor.Magenta;
        Secondary_ForegroundColor = ConsoleColor.Magenta;
        Accent_BackgroundColor = ConsoleColor.Magenta;
        Accent_ForegroundColor = ConsoleColor.Magenta;
    }



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