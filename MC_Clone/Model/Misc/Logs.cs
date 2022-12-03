using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MC_Clone;
internal class Logs
{
    private string m_exePath = string.Empty;
    public Logs(string logMessage, string title)
    {
        LogWrite(logMessage, title);
    }
    public Logs(string logMessage)
    {
        LogWrite(logMessage);
    }
    public Logs(List<string> logMessage)
    {
        LogList(logMessage);
    }
        
    public void LogWrite(string logMessage)
    {
        m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        try
        {
            using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
            {
                LogFormat(logMessage, w);
            }
        }
        catch (Exception ex)
        {
        }
    }
    public void LogWrite(string logMessage, string title) //find better solution (one method)
    {
        m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        try
        {
            using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
            {
                Log(logMessage,title, w);
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void LogFormat(string logMessage, TextWriter txtWriter)
    {
        try
        {
            txtWriter.Write("\r\nLog Entry : ");
            txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            txtWriter.WriteLine("  :{0}", logMessage);
            txtWriter.WriteLine("-------------------------------");
        }
        catch (Exception ex)
        {
        }
    }
    public void SimpleLog(string logMessage, TextWriter txtWriter)
    {
        try
        {
            string time = DateTime.Now.ToLongTimeString();
            string date = DateTime.Now.ToLongDateString();
            txtWriter.Write($"\r\nLog Entry : {time}/{date}");
            txtWriter.WriteLine($"MSG: '{logMessage}'");
            txtWriter.WriteLine("-------------------------------");
        }
        catch (Exception ex)
        {
        }
    }
    public void Log(string logMessage, string title, TextWriter txtWriter)
    {
        try
        {
            txtWriter.WriteLine($"{title}: ");
            txtWriter.WriteLine($"{logMessage}");
            txtWriter.WriteLine("--- --- --- --- --- --- ");

        }
        catch (Exception ex)
        {
        }
    }
    public void LogList(List<string> list)
    {
        m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        

        try
        {
            using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
            {
                foreach (string item in list)
                {
                    w.WriteLine(item);
                }
                w.WriteLine("-------------------------------");
            }
        }
        catch (Exception ex)
        {
        }
    }
}
