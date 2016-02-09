using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dialer
{
    [XmlRoot("settings")]
    public class Settings
    {
        [XmlElement("dbserver")]
        public dbserver DBserver { get; set; }
        [XmlElement("service")]
        public service Service { get; set; }
        [XmlElement("popurl")]
        public popurl PopURL { get; set; }
        [XmlElement("wrapup")]
        public int WrapUp { get; set; }
        [XmlElement("listlength")]
        public int ListLength{ get; set; }
    }

    [XmlRoot("dbserver")]
    public class dbserver
    {
        [XmlElement("ip")]
        public ip IP { get; set; }
        [XmlElement("dbname")]
        public string dbname { get; set; }
        [XmlElement("user")]
        public string user { get; set; }
        [XmlElement("password")]
        public string password { get; set; }
    }

    [XmlRoot("ip")]
    public class ip
    {
        [XmlAttribute("port")]
        public string port { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    [XmlRoot("service")]
    public class service
    {
        [XmlAttribute("bindip")]
        public string bindip { get; set; }
        [XmlAttribute("bindport")]
        public string bindport { get; set; }
    }

    [XmlRoot("popurl")]
    public class popurl
    {
        [XmlAttribute("address")]
        public string address { get; set; }
        [XmlText]
        public bool Value { get; set; }
    }
}
