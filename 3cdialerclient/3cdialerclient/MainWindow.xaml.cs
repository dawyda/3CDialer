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
            if (File.Exists("\\\\?\\" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\3CdialerClient\\settings.xml"))
            {}
            else
            {
                Settings.FirstRun();
            }
            serverHandler = new ServerHandler();
        }

        private void btnLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	//get login then send credentials via socket to server with method name as login
            bool success = serverHandler.Login(txt_username.Text, txt_password.Password);
            if (!success)
            {
                tb_error.Visibility = System.Windows.Visibility.Visible;
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
