using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace _cdialerclient
{
    class ClientSettingsHandler
    {
        internal static ClientSettings GetSettings()
        {
            ClientSettings settings = null;
            try
            {
                string settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\3CdialerClient\\settings.xml";
                StreamReader sr = new StreamReader(settingsPath);
                settings =  (ClientSettings)new XmlSerializer(typeof(ClientSettings)).Deserialize(sr);
                sr.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Setting read error: " + e.Message);
            }
            return settings;
        }

        internal static bool SetSettings(ClientSettings settings)
        {
            bool done = false;
            try
            {
                string settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\3CdialerClient\\settings.xml";
                StreamWriter sw = new StreamWriter(settingsPath);
                new XmlSerializer(typeof(ClientSettings)).Serialize(sw,settings);
                sw.Close();
                done = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Setting read error: " + e.Message);
            }
            return done;
        }
    }
}
