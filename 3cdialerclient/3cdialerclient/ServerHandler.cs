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
        internal string userCampaign;
        internal List<Call> calls;
        private string token;
        private string userid;
        
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
            if(server.GET(GetXMLString(logins)))
            {
                //get login response instance
                LoginResponse response = (LoginResponse) GetObjectfromXML(server.responseXml,typeof(LoginResponse));
                if (response.args.Response == true)
                {
                    userid = response.args.Userid;
                    token = response.args.Token;
                    userCampaign = response.args.Campaign;
                    logged = true;
                }
            }
            return logged;
        }

        //request call list
        public bool RequestCallList()
        {
            bool requested = false;
            ListRequest lr = new ListRequest();
            lr.Method = "GetCallList";
            lr.Session = new session() { Userid = this.userid, Token = this.token };
            lr.Args.Ext = ClientSettingsHandler.GetSettings().Extension;
            if(server.GET(GetXMLString(lr)))
            {
                requested = true;
            }
            return requested;
        }

        //general funcs
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
