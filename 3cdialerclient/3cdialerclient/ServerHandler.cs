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
        internal Call CurrentCall;
        private string token;
        private string userid;
        internal bool endReached;
        //string error = "";
        
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
            if(server.GET("L:" + GetXMLString(logins)))
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
        public bool Logout()
        {
            LogoutReq lq = new LogoutReq() { Session = new session() { Userid = this.userid, Token = this.token } };
            if (server.GET("O:" + GetXMLString(lq)))
            {
                return true;
            }
            else
            {
                Logger.Log("Socket client error: failed to initiate logout.");
                return false;
            }
        }

        //request call list
        public bool RequestCallList()
        {
            bool requested = false;
            endReached = false;
            ListRequest lr = new ListRequest();
            lr.Method = "GetCallList";
            lr.Session = new session() { Userid = this.userid, Token = this.token };
            lr.Args = new ReqArgs() { Ext = ClientSettingsHandler.GetSettings().Extension, Campaign = this.userCampaign};
            if(server.GET("C:" + GetXMLString(lr)))
            {
                calls = ((CallListXML)GetObjectfromXML(server.responseXml,typeof(CallListXML))).Args.Calls.ToList<Call>();
                //set current call;
                CurrentCall = calls[0];
                //acknowledge receipt
                server.GET("AL:" + GetXMLString(new AckRequest() { Session = new session() { Userid = this.userid, Token = this.token } }));
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
            Object obj = null;
            try
            {
                StringReader sw = new StringReader(xmlString);
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(t);
                obj = xs.Deserialize(sw);
            }
            catch (Exception e)
            {
                Logger.Log("Deserialization failed. Server might have closed." + e.Message);
            }
            return obj;
        }
    }
}
