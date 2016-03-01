using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
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
            tb_SetStatus.Text = "Note: After login some settings changes might require you login/logout this client.";
        }
        //when user runs program for the first time
        public static void FirstRun()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\3CdialerClient";
                Directory.CreateDirectory(path);
                //set folder permissions to everyone.
                //DirectoryInfo info = new DirectoryInfo(path);
                //DirectorySecurity drule = info.GetAccessControl();
                //drule.AddAccessRule(new FileSystemAccessRule(new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                //info.SetAccessControl(drule);
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
                try
                {
                    using (StreamWriter sr = File.AppendText("RunLog.txt"))
                    {
                        sr.WriteLine("Failed to write default settings.\nTry running as admin.\nExtra info:\n" + e.Message, "3CDialer Client Init Error");
                    }
                }
                catch { }
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