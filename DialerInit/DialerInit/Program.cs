using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DialerInit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is 3CDialer initializer wizard.");
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Enter the selection number to perfom action:");
            Console.WriteLine("Items Menu:\n Enter: ");
            Console.WriteLine("1. To add plugin to softphone.\n2. Set default settings for client.\n3. To exit.");
            while (true)
            {
                Console.Write(">");
                string input = Console.ReadLine();
                if( input == "1"){
                     setPlugin();
                }
                else if (input == "2")
                {
                    setSettings();
                }
                else if (input == "3")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("\nInput not recognized.\n");
                }
            }
            //Console.WriteLine("Press ENTER to exit...");
            //Console.ReadKey();
        }

        private static void setSettings()
        {
            string ip;
            string port;
            string ext;
            string setF = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"+
                "<settings xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"+
                  "<server ip=\"{0}\" port=\"{1}\" />"+
                  "<retry>3</retry>"+
                  "<extension>{2}</extension>"+
                "</settings>";
            Console.WriteLine("\n We need some information :)");
            Console.Write("What is the IP address of the server where the dialer service is installed?\n>");
            ip = Console.ReadLine();
            Console.Write("What is the port used by the dialer service?[15500]\n>");
            port = Console.ReadLine();
            Console.Write("What is the agent's extension number?(optional)\n>");
            ext = Console.ReadLine();
            Console.WriteLine("OK\nCreating settings.xml file for client.");
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\3CdialerClient\settings.xml";
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\3CdialerClient"))
            {
                try
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\3CdialerClient");
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception occured: make sure you have rights to create files/ Run app as admin");
                }
            }
            //checking for nulls and applying defaults.
            ip = ip == "" ? "127.0.0.1" : ip;
            port = port == "" ? "15500" : port;
            ext = ext == "" ? "100" : ext;

            File.WriteAllText(path, string.Format(setF,ip,port,ext));
            Console.WriteLine("\nSettings initialized successfully!\nEnter 3 to exit...");
        }

        private static void setPlugin()
        {
            string path = @"C:\ProgramData\3CXPhone for Windows\PhoneApp\3CXWin8Phone.exe.config";
            if (!File.Exists(path))
            {
                Console.WriteLine("Error. Details:\n i. You are using v14 softphone\nii. You have not installed softphone.");
                return;
            }
            string line;
            string text = File.ReadAllText(path);
            Console.WriteLine("Exit softphone then press ENTER to continue...");
            Console.ReadKey();

            using (StreamReader stream = new StreamReader(path))
            {
                while ((line = stream.ReadLine()) != null)
                {
                    if (line.IndexOf("<add key=\"CRMPlugin\" value=\"CallNotifier") > -1)
                    {
                        break;
                    }
                }
            }

            if(text.IndexOf(",PluginLoader") > -1){
                Console.WriteLine("DLL has already been added. Aborting operation...");
                return;
            }
            Console.WriteLine("Line found...adding DLL to load.\n-------------------------------------------------");
            string repline = line.Substring(0, (line.Length - 3)) + ",PluginLoader\"/>";
            text = text.Replace(line, repline);
            File.WriteAllText(path, text);
            Console.WriteLine("Plugin added!\n Launch softphone first the client to use dialer.\n\n Enter 3 to exit:");
        }
    }
}
