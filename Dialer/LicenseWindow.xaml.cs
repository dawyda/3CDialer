﻿using System;
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
using Microsoft.Win32;

namespace Dialer
{
	/// <summary>
	/// Interaction logic for LicenseWindow.xaml
	/// </summary>
	public partial class LicenseWindow : Window
	{
        string DEMO_KEY = "1234-ABCD-ZYXW-9876";
        string keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\3CDialer";
        string DEMO_FILE_PATH = Environment.CurrentDirectory + @"\dlic.txt";

		public LicenseWindow()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}

		private void btn_ActivateDemo_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			//check if demo license file exists.
            if (System.IO.File.Exists(DEMO_FILE_PATH))
            {
                MessageBox.Show("Previous demo activation detected.","Demo Activation Error!");
                e.Handled = true;
                return;
            }
            //create in registry and set status as DEMO.
            string valueName = "KeyType";
            try
            {
                if (Registry.GetValue(keyName, valueName, null) == null)
                {
                    RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE", true).CreateSubKey("3CDialer");
                    key.SetValue("KeyType", "Demo");
                    key.Close();
                }
                Registry.SetValue(keyName, valueName, "Demo");
                Registry.SetValue(keyName, "KeyCode", DEMO_KEY);
                Registry.SetValue(keyName, "NumUsers", 5);
                Registry.SetValue(keyName, "Status", "Demo Activated");
            }
            catch (Exception ex)
            {
                Logger.LogError("Demo activation failed. Contact administrator/support! \n Info: " + ex.InnerException + " : " + ex.Message);
            }
            //create file with demo activation date.
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.File.Create(DEMO_FILE_PATH)))
            {
                sw.WriteLine(DateTime.Now.ToLongDateString());
                sw.Close();
            }
            try
            {
                System.IO.File.SetAttributes(DEMO_FILE_PATH, System.IO.File.GetAttributes(DEMO_FILE_PATH) | System.IO.FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            this.Close();
		}

		private void btn_ActivateKey_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            
            //stuff to do activation
		}
	}
}