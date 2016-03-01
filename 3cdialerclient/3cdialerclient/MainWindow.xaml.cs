using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _cdialerclient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServerHandler serverHandler;
        public MainWindow()
        {
            InitializeComponent();
            //if first run
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\3CdialerClient\settings.xml";
            if (!File.Exists(path))
            {
                Settings.FirstRun();
                //run initapp with args 12
                RunInitApp();
            }
            serverHandler = new ServerHandler();
            txt_username.Focus();
        }

        private void RunInitApp()
        {
            setPlugin();
        }

        private static void setPlugin()
        {
            string path = @"C:\ProgramData\3CXPhone for Windows\PhoneApp\3CXWin8Phone.exe.config";
            if (!File.Exists(path))
            {
                //Console.WriteLine("Error. Details:\n i. You are using v14 softphone\nii. You have not installed softphone.");
                return;
            }
            string line;
            string text = File.ReadAllText(path);
            //Console.WriteLine("Exit softphone then press ENTER to continue...");
            //Console.ReadKey();
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
                Logger.Log("Failed to add pluging to softphone. Reinstall the softphone and try again.");
                return;
            }

            if (line.IndexOf(",PluginLoader") > -1)
            {
                //Console.WriteLine("DLL has already been added. Aborting operation...");
                try
                {
                    File.Copy("PluginLoader.dll", @"C:\ProgramData\3CXPhone for Windows\PhoneApp\PluginLoader.dll", true);
                }
                catch (Exception e)
                {
                    //Console.WriteLine("X! Error occured check log file....");
                    //Thread.Sleep(1000);
                   Logger.Log(e.Message);
                }
                return;
            }
            //Console.WriteLine("Line found...adding DLL to load.\n-------------------------------------------------");
            string repline = line.Substring(0, (line.Length - 3)) + ",PluginLoader\"/>";
            text = text.Replace(line, repline);
            File.WriteAllText(path, text);
            File.Copy(@"Assets\PluginLoader.dll", @"C:\ProgramData\3CXPhone for Windows\PhoneApp\PluginLoader.dll", true);
            //Console.WriteLine("Plugin added!\n Launch softphone first the client to use dialer.\n\n Enter 3 to exit:");
            //Thread.Sleep(1000);
        }

        private void btnLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	//get login then send credentials via socket to server with method name as login
            string user = txt_username.Text;
            string password = txt_password.Password;
            tb_error.Text = "Wrong username or password.";
            tb_error.Visibility = System.Windows.Visibility.Hidden;

            if (user == "" || password == "")
            {
                tb_error.Text = "No blanks allowed. Fill in details.";
                tb_error.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            bool success = serverHandler.Login(txt_username.Text, txt_password.Password);
            if (!success)
            {
                tb_error.Visibility = System.Windows.Visibility.Visible;
                tb_error.Text = serverHandler.error;
                txt_password.SelectAll();
                txt_password.Focus();
                e.Handled = true;
                return;
            }
            Window win = new AgentWindow(serverHandler);
            win.Show();
            this.Close();
        }

        private void btnCancelLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Window settings = new Settings();
            settings.ShowDialog();
        }
    }
}
