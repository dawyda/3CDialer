using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace _cdialerclient
{
    class Logger
    {
        public Logger() { }
        public static void Log(string msg)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\3CdialerClient\log.txt";
            StreamWriter sr = File.AppendText(path);
            sr.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), msg);
            sr.Close();
        }
    }
}
