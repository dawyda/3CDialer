using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace _cdialerclient
{
    [XmlRoot("3cdialerclient",Namespace="",IsNullable=true)]
    public class LoginXML
    {
        [XmlElement("method")]
        public string Method { get; set; }
        [XmlElement("session")]
        public session Session { get; set; }
        [XmlElement("args")]
        public args Args { get; set; }
    }
    [XmlRoot("session")]
    public class session
    {
        [XmlElement("userid")]
        public string Userid { get; set; }
        [XmlElement("token")]
        public string Token { get; set; }
    }
    [XmlRoot("args")]
    public class args
    {
        [XmlElement("username")]
        public string Username { get; set; }
        [XmlElement("password")]
        public string Password { get; set; }
    }

    /**
     * Section for login response; 
     */
    [XmlRoot("3cdialerserver")]
    public class LoginResponse
    {
        [XmlElement("method")]
        public string Method { get; set; }
        [XmlElement("args")]
        public args RespArgs { get; set; }
    }

    [XmlRoot("args")]
    public class RespArgs
    {
        [XmlElement("response")]
        public bool Response { get; set; }
        [XmlElement("userid")]
        public string Userid { get; set; }
        [XmlElement("token")]
        public string Token { get; set; }
    }
}
