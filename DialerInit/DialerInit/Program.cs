using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DialerInit
{
    class Program
    {
        private static string menuString;
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "1":
                        setPlugin();
                        break;
                    case "2":
                        setClientSettings();
                        break;
                    case "12":
                        setClientSettings();
                        setPlugin();
                        break;
                    case "4":
                        installDialerService();
                        break;
                    default:
                        Log("Args not defined in function list for client settings.");
                        break;
                }
                return;
            }
            menuString = "1. To add plugin to softphone.\n" +
                "2. Set default settings for client.\n" +
                "3. Set settings for server application.\n" +
                "4. Install dialer service on this computer (make sure to have run AS ADMIN).\n" +
                "5. To setup dialer database (Import tables).\n" +
                "6. To uninstall the dialer service.\n"+
                "7. To exit.\n";
                
            while (true)
            {
                Console.Clear();
                Console.WriteLine("This is 3CDialer initializer wizard. We have made things easier :)");
                Console.WriteLine("-----------------------------------------------------------------\n");
                Console.WriteLine("Enter the item number to perfom action:");
                Console.WriteLine("Items Menu:\nEnter:\n ");
                Console.WriteLine(menuString);
                Console.Write(">");
                string input = Console.ReadLine();
                if( input == "1"){
                     setPlugin();
                }
                else if (input == "2")
                {
                    setClientSettings();
                }
                else if (input == "3")
                {
                    setServerSettings();
                }
                else if (input == "4")
                {
                    installDialerService();
                }
                else if (input == "5")
                {
                    importDbTables();
                }
                else if (input == "6")
                {
                    uninstallService();
                }
                else if (input == "7")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Input not recognized. Enter number in selection only.\n");
                    Thread.Sleep(500);
                }
            }
            Console.WriteLine("Exiting...");
            Thread.Sleep(700);
            //Console.ReadKey();
        }

        private static void uninstallService()
        {
            Console.WriteLine("Uninstalling service...please wait...");
            System.Diagnostics.Process.Start("cmd.exe /c \"sc delete DialerService\"");
            Thread.Sleep(1000);
            Console.WriteLine("Success...Done!");
            Thread.Sleep(1500);
        }

        private static void installDialerService()
        {
            using (System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController("DialerService"))
            {
                try{
                    if (service.Status.ToString() != null)
                    {
                        Console.WriteLine("Dialer Service already installed!...");
                        Thread.Sleep(1500);
                        return;
                    }
                }
                catch(Exception)
                {
                }
            }

            if (!File.Exists(@"C:\ProgramData\3CDialer\settings.xml"))
            {
                Console.WriteLine("Set default server settings first...");
                Thread.Sleep(2500);
                return;
            }

            string cmdLoc = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /i ";
            System.Diagnostics.Process cmd = new System.Diagnostics.Process();

            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.Arguments = "/c \""+ cmdLoc +"\" Assets\\DialerService.exe";

            try
            {
                cmd.Start();
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                Console.WriteLine("Command executed successfully!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Log(e.Message);
                Console.WriteLine("Failed to install service. Make sure you are admin!");
            }

            /* execute "dir" */

            //cmd.StandardInput.WriteLine("dir");
            Thread.Sleep(1500);
        }

        private static void setServerSettings()
        {
            string DBIp =null;
            string user = null;
            string pass = null;
            string database = null;
            string port = null;
            Console.WriteLine("We need you to provide the following information:\nWhat is you Database server IP address?\n>");
            DBIp = Console.ReadLine();
            Console.WriteLine("DB username?[Default=root]\n>");
            user = Console.ReadLine();
            Console.WriteLine("DB user password?\n>");
            pass = Console.ReadLine();
            Console.WriteLine("Database name?[Default=3cdialer]\n>");
            database = Console.ReadLine();
            DBIp = DBIp == "" ? "127.0.0.1" : DBIp;
            user = user == "" ? "root" : user;
            pass = pass == "" ? "" : pass;
            database = database == "" ? "3cdialer" : database;
            port = port == "" ? "3306" : port;
            string templateStr = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"+
                        "<settings xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">"+
                          "<dbserver>"+
                            "<ip port=\"{0}\">{1}</ip>"+
                            "<dbname>{2}</dbname>"+
                            "<user>{3}</user>"+
                            "<password>{4}</password>"+
                          "</dbserver>"+
                          "<service bindip=\"0.0.0.0\" bindport=\"15500\" />"+
                          "<popurl address=\"\">false</popurl>"+
                          "<wrapup>10</wrapup>"+
                          "<listlength>20</listlength>"+
                        "</settings>";
            string path = @"C:\ProgramData\3CDialer\settings.xml";
            if (!Directory.Exists(@"C:\ProgramData\3CDialer\"))
            {
                try
                {
                    Directory.CreateDirectory(@"C:\ProgramData\3CDialer\");
                    GrantAccess(@"C:\ProgramData\3CDialer");
                }
                catch (Exception)
                {
                    Console.WriteLine("Exception occured: make sure you have rights to create files/ Run app as admin");
                }
            }
            File.WriteAllText(path, string.Format(templateStr, port, DBIp, database, user, pass));
            Console.WriteLine("\nDefault server settings initialized successfully!");
            Thread.Sleep(1000);
        }

        private static void importDbTables()
        {
            //add for getting host,user and pass.
            string path = @"Assets\3cdialer.sql";
            System.Diagnostics.Process p;
            try{
                p = System.Diagnostics.Process.Start("mysqldump -u root --password=toor 3cdialer <" + path);
                if (p.HasExited)
                {
                    Console.WriteLine("Databases have been imported successfully!\n");
                }
            }
            catch(Exception e)
            {
                Log(e.Message);
            }
            Thread.Sleep(1500);
        }

        private static void setClientSettings()
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
                    GrantAccess(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\3CdialerClient");
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
            Console.WriteLine("\nSettings initialized successfully!");
            Thread.Sleep(1000);
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
            bool linefound = false;

            using (StreamReader stream = new StreamReader(path))
            {
                while ((line = stream.ReadLine()) != null)
                {
                    if (line.IndexOf("<add key=\"CRMPlugin\" value=\"CallNotifier") > -1)
                    {
                        linefound = true;
                        break;
                    }
                }
            }
            if (!linefound)
            {
                Console.WriteLine("Failed to add pluging to softphone. Reinstall the softphone and try again.");
                return;
            }

            if(line.IndexOf(",PluginLoader") > -1){
                Console.WriteLine("DLL has already been added. Aborting operation...");
                try
                {
                    File.Copy(@"Assets\PluginLoader.dll", @"C:\ProgramData\3CXPhone for Windows\PhoneApp\PluginLoader.dll", true);
                }
                catch(Exception e){
                    Console.WriteLine("X! Error occured check log file....");
                    Thread.Sleep(1000);
                    Log(e.Message);
                }
                return;
            }
            Console.WriteLine("Line found...adding DLL to load.\n-------------------------------------------------");
            string repline = line.Substring(0, (line.Length - 3)) + ",PluginLoader\"/>";
            text = text.Replace(line, repline);
            File.WriteAllText(path, text);
            File.Copy(@"Assets\PluginLoader.dll", @"C:\ProgramData\3CXPhone for Windows\PhoneApp\PluginLoader.dll",true);
            Console.WriteLine("Plugin added!\n Launch softphone first the client to use dialer.\n\n Enter 3 to exit:");
            Thread.Sleep(1000);
        }
        private static void Log(string Msg)
        {
            File.WriteAllText("error.txt",Msg);
        }
        private static bool GrantAccess(string path)
        {
            bool granted = false;
            //grant your access here.
            try
            {
                DirectoryInfo info = new DirectoryInfo(path);
                System.Security.AccessControl.DirectorySecurity drule = info.GetAccessControl();
                drule.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null), System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.InheritanceFlags.ObjectInherit | System.Security.AccessControl.InheritanceFlags.ContainerInherit, System.Security.AccessControl.PropagationFlags.NoPropagateInherit, System.Security.AccessControl.AccessControlType.Allow));
                info.SetAccessControl(drule);
                return granted = true;
            }
            catch(Exception e)
            {
                Log("Failed to grant access. " + e.Message);
            }
            return granted;
        }
    }
}
