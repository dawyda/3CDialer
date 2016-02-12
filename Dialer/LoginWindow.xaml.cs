using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dialer
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
        private DBHandler database;
		public LoginWindow()
		{
            InitializeComponent();
			Settings settings = (Settings) new System.Xml.Serialization.XmlSerializer(typeof(Settings)).Deserialize(System.IO.File.OpenRead(@"C:\ProgramData\3CDialer\settings.xml"));
            string connstr = "SERVER=" + settings.DBserver.IP.Value +
                ";PORT=" + settings.DBserver.IP.port +
                ";DATABASE=" + settings.DBserver.dbname +
                ";UID=" + settings.DBserver.user +
                ";PASSWORD=" + settings.DBserver.password + ";";
            database  = new DBHandler(connstr);
            txt_username.Focus();
		}

		private void DoLogin(object sender, System.Windows.RoutedEventArgs e)
		{
            if(txt_username.Text == "" || pwd_password.Password == "")
            {
                tb_statusLogin.Text = "No blanks allowed!!!";
                return;
            }
            LoginResp success = database.AdminLogin(txt_username.Text, pwd_password.Password);
            if (!success.success)
            {
                tb_statusLogin.Text = "Invalid username/password.";
                txt_username.Focus();
            }
            else
            {
                MainWindow server = new MainWindow(success.name);
                server.Show();
                database = null;
                this.Close();
            }
		}
	}
}