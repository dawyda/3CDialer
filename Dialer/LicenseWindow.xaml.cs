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
	/// Interaction logic for LicenseWindow.xaml
	/// </summary>
	public partial class LicenseWindow : Window
	{
		public LicenseWindow()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

		private void btn_ActivateDemo_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			// TODO: Add event handler implementation here.
		}

		private void btn_ActivateKey_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            Properties.Settings.Default["company"] = txt_company.Text;
            Properties.Settings.Default["person"] = txt_person.Text;
            Properties.Settings.Default["email"] = txt_email.Text;
            Properties.Settings.Default["keycode"] = txt_keycode.Text;
            Properties.Settings.Default.Save();
            lbl_actStatus.Content = "Contacting server...";
            //stuff to do activation
		}
	}
}