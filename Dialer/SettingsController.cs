using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dialer
{
    public class SettingsController : ViewModelEntity
    {
        protected Settings _settings;
        public SettingsController()
        {
            //read initial settings from xml file
            try
            {
                StreamReader sr = new StreamReader(@"C:\ProgramData\3CDialer\settings.xml");
                XmlSerializer xmsr = new XmlSerializer(typeof(Settings));
                _settings = (Settings)xmsr.Deserialize(sr);
                sr.Close();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        public Settings Settings
        {
            get { return _settings; }
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    NotifyPropertyChanged("Settings");
                }
            }
        }

        public bool SaveSettings()
        {
            bool saved = false;
            try
            {
                StreamWriter sr = new StreamWriter(@"C:\ProgramData\3CDialer\settings.xml");
                XmlSerializer xmsw = new XmlSerializer(typeof(Settings));
                xmsw.Serialize(sr, _settings);
                sr.Close();
                saved = true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return saved;
        }
    }
}
