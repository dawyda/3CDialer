using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Dialer
{
    class Logger
    {
        public Logger() { }
        public static void LogError(string msg)
        {
            StreamWriter sr = File.AppendText(@"D:\3CDialerErrorLog.txt");
            sr.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), msg);
            sr.Close();
        }
        public static void LogServiceError(string msg)
        {
        }
        public static void LogDBError(string msg)
        {
            StreamWriter sr = File.AppendText(@"D:\3CDialerDBLog.txt");
            sr.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), msg);
            sr.Close();
        }
    }
}
