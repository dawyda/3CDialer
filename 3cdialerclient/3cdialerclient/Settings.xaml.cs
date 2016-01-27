using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace _cdialerclient
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : Window
	{
        ClientSettings settings;
        string settingsPath;
        XmlSerializer xs;
		public Settings()
		{
			this.InitializeComponent();
            settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\3CdialerClient";
            xs = new XmlSerializer(typeof(ClientSettings));
            InitSettings();
		}

        private void InitSettings()
        {
            settings = ClientSettingsHandler.GetSettings();
            txt_ip.Text = settings.Server.ip;
            txt_port.Text = settings.Server.port;
            txt_retry.Text = settings.Retry.ToString();
            txt_extension.Text = settings.Extension;
        }
        //when user runs program for the first time
        public static void FirstRun()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\3CdialerClient";
                Directory.CreateDirectory(path);
                //create file and enter the default settings;
                FileStream fs = File.Create(path + "\\settings.xml");
                XmlSerializer xs = new XmlSerializer(typeof(ClientSettings));
                xs.Serialize(fs, new ClientSettings() { Server = new server() { ip = "127.0.0.1", port = "15500" }, Retry = 3, Extension = "" });
                fs.Close();
                fs = null;
                fs = File.Create(path + "\\log.txt");
                fs.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Failed to write default settings.\nTry running as admin.\nExtra info\n" + e.Message,"3CDialer Client Init Error");
            }
        }

        private void btn_saveSettings_Click(object sender, RoutedEventArgs e)
        {
            settings.Server.ip = txt_ip.Text;
            settings.Server.port = txt_port.Text;
            settings.Retry = Convert.ToInt32(txt_retry.Text);
            settings.Extension = txt_extension.Text;
            if (!ClientSettingsHandler.SetSettings(settings))
            {
                tb_SetStatus.Text = "Save FAILED!!";
            }
            tb_SetStatus.Text = "Saved!!";
        }

        private void btnCancelSettings_Click(object sender, RoutedEventArgs e)
        {
            tb_SetStatus.Text = "user cancelled!!";
            settings = ClientSettingsHandler.GetSettings();
            txt_ip.Text = settings.Server.ip;
            txt_port.Text = settings.Server.port;
            txt_retry.Text = settings.Retry.ToString();
            txt_extension.Text = settings.Extension;
            this.Close();
        }

        private void btnSettingsClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
	}
}