using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DialerService;

namespace _cdialerclient
{
    [XmlRoot("request")]
    public class ListRequest
    {
        [XmlElement("method")]
        public string Method { get; set; }
        [XmlElement("session")]
        public session Session { get; set; }
        [XmlElement("args")]
        public ReqArgs Args { get; set; }
    }

    [XmlRoot("args")]
    public class ReqArgs
    {
        [XmlElement("ext")]
        public string Ext { get; set; }
        [XmlElement("campaign")]
        public string Campaign { get; set; }
    }
}
