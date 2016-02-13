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
            using (
            StreamWriter sr = File.AppendText(@"C:\ProgramData\3CDialer\ErrorLog.txt"))
            {
                sr.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), msg);
                sr.Close();
            }
        }
        public static void LogServiceError(string msg)
        {
            using (StreamWriter sr = File.AppendText(@"C:\ProgramData\3CDialer\3CDialerServiceLog.txt"))
            {
                sr.AutoFlush = true;
                sr.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), msg);
                sr.Close();
            }
        }
        public static void LogActivity(string msg)
        {
            using (StreamWriter sr = File.AppendText(@"C:\ProgramData\3CDialer\Activity.txt"))
            {
                sr.AutoFlush = true;
                sr.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), msg);
                sr.Close();
            }
        }
        public static void LogDBError(string msg)
        {
            using (StreamWriter sr = File.AppendText(@"C:\ProgramData\3CDialer\DBErrorLog.txt"))
            {
                sr.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), msg);
                sr.Close();
            }
        }
        public static void LogSocket(string msg)
        {
            using (StreamWriter sr = File.AppendText(@"C:\ProgramData\3CDialer\SocketLog.txt"))
            {
                sr.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), msg);
                sr.Close();
            }
        }
        public static void Log(string msg)
        {
            using (StreamWriter sr = File.AppendText(@"C:\ProgramData\3CDialer\GeneralLog.txt"))
            {
                sr.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), msg);
                sr.Close();
            }
        }
    }
}
