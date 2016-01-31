using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using _cdialerclient;

namespace _cdialerclient
{
    [XmlRoot("ackresponse")]
    public class AckResponse
    {
        [XmlElement("args")]
        public bool Acknowledged { get; set; }
    }
    [XmlRoot("ackrequest")]
    public class AckRequest
    {
        [XmlElement("session")]
        public session Session { get; set; }
    }
}
