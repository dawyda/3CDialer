using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _cdialerclient
{
    /**
     *This class will handle all server socket related stuff.
     * It will contain functions like login,get list, update, refesh, etc.
     **/
    public class ServerHandler
    {
        protected ServerSocket server;
        
        public ServerHandler() {
            server = new ServerSocket();        
        }
        public bool Login(string username, string password)
        {
            bool logged = false;
            LoginXML logins = new LoginXML();
            logins.Method = "Login";
            logins.Session = new session() { Userid = string.Empty, Token= string.Empty};
            logins.Args = new args() { Username = username, Password = password};
            if(server.SendLogins(GetXMLString(logins)))
            {
                //get login response instance
                LoginResponse response = (LoginResponse) GetObjectfromXML(server.responseXml,typeof(LoginResponse));
                logged = true;
            }
            return logged;
        }
        private string GetXMLString<T>(T arg)
        {
            StringWriter sw = new StringWriter();
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(arg.GetType());
            System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            ns.Add("", "");
            xs.Serialize(sw, arg, ns);
            System.Diagnostics.Trace.WriteLine(sw.ToString());
            return sw.ToString();
        }
        private Object GetObjectfromXML(string xmlString, Type t)
        {
            StringReader sw = new StringReader(xmlString);
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(t);
            return xs.Deserialize(sw);
        }
    }
}
