using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace _cdialerclient
{
    [XmlRoot("logoutReq")]
    public class LogoutReq
    {
        [XmlElement("session")]
        public session Session { get; set; }
        //[XmlElement("method")]
        //public string Method { get; set; }
    }
    [XmlRoot("logoutAck")]
    public class LogoutResponse
    {
        [XmlElement("response")]
        public bool Response { get; set; }
        //[XmlElement("method")]
        //public string Method { get; set; }
    }
}
