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
	/// Interaction logic for AdminPasswordWindow.xaml
	/// </summary>
	public partial class AdminPasswordWindow : Window
	{
		private DBHandler handle;
		public AdminPasswordWindow(string conn)
		{
			this.InitializeComponent();
            this.handle = new DBHandler(conn);
			// Insert code required on object creation below this point.
		}

		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
			// TODO: Add event handler implementation here.
		}

		private void OK_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            if (pwd_pass1.Password == "" || pwd_pass2.Password == "")
            {
                tb_passStatus.Text = "Blanks not allowed!!";
                return;
            }
            else if (!(pwd_pass1.Password == pwd_pass2.Password))
            {
                tb_passStatus.Text = "Passwords don't match!!";
                return;
            }
            else if (pwd_pass1.Password.Length < 6)
            {
                tb_passStatus.Text = "Password is too short(6 chars)";
                return;
            }
            else
            {
                handle.ChangeAdminPassword(pwd_pass1.Password);
                tb_passStatus.Text = "Password changed!!";
                tb_passStatus.Foreground = new SolidColorBrush(Colors.Green);
                long i  = 2400000000;
                while (i > 0)
                {
                    i--;
                }
            }
            this.Close();
		}
	}
}