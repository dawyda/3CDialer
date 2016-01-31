using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace _cdialerclient
{
    [XmlRoot("dialerserver")]
    public class CallListXML
    {
        [XmlElement("session")]
        public session Session { get; set; }
        [XmlElement("method")]
        public string Method { get; set; }
        [XmlElement("args")]
        public CallArgs Args { get; set; }
    }

    [XmlRoot("args")]
    public class CallArgs
    {
        [XmlElement("calllist")]
        public Call[] Calls { get; set; }
    }

    [XmlRoot("call")]
    public class Call
    {
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlElement("name")]
        public string name { get; set; }
        [XmlElement("tel1")]
        public string tel1 { get; set; }
        [XmlElement("tel2")]
        public string tel2 { get; set; }
        [XmlElement("lang")]
        public string lang { get; set; }
        [XmlElement("country")]
        public string country { get; set; }
        [XmlElement("custom1")]
        public string custom1 { get; set; }
        [XmlElement("custom2")]
        public string custom2 { get; set; }
        [XmlElement("custom3")]
        public string custom3 { get; set; }
        [XmlElement("custom4")]
        public string custom4 { get; set; }
        [XmlElement("custom5")]
        public string custom5 { get; set; }
        [XmlElement("custom6")]
        public string custom6 { get; set; }
        [XmlElement("custom7")]
        public string custom7 { get; set; }
    }
}
