using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace _cdialerclient
{
    [XmlRoot("settings")]
    public class ClientSettings
    {
        [XmlElement("server")]
        public server Server { get;set;}
        [XmlElement("retry")]
        public int Retry { get; set; }
        [XmlElement("extension")]
        public string Extension { get; set; }
    }
    [XmlRoot("server")]
    public class server
    {
        [XmlAttribute("ip")]
        public string ip { get; set; }
        [XmlAttribute("port")]
        public string port { get; set; }
    }
}
