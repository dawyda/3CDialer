using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace _cdialerclient
{
    [XmlRoot("3cdialerclient")]
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
    }
}
