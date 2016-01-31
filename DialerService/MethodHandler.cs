using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using _cdialerclient;
using System.Xml;

namespace DialerService
{
    class MethodHandler
    {
        protected DBHandler dbhandler = new DBHandler();
        string response;
        //excutes specified method in xml
        public string Execute(string xmlString)
        {
            response = "error";
            string[] strings = xmlString.Split(':');
            switch(strings[0])
            {   //login
                case "L":
                    response = DoLogin(strings[1]);
                    break;
                //list request
                case "C":
                    response = GetCalls(strings[1]);
                    break;
                //acknowledge list was received
                case "AL":
                    response = AcknowledgeCalls(strings[1]);
                    break;
                //update list
                case "UL":
                    response = GetCalls(strings[1]);
                    break;
                //logout
                case "O":
                    response = DoLogout(strings[1]);
                    break;
                default:
                    //unmatched method;
                    return "error";
            }
            return response;
        }

        private string DoLogout(string logoutXml)
        {
            LogoutReq lr = GetObjectfromXML(logoutXml,typeof(LogoutReq)) as LogoutReq;
            LogoutResponse la = new LogoutResponse() { Response = false};
            if (dbhandler.SessionCheck(lr.Session.Token))
            {
                la.Response = dbhandler.Logout(lr.Session);
            }
            return GetXMLString(la);
        }

        private string AcknowledgeCalls(string ackXml)
        {
            AckRequest aq = GetObjectfromXML(ackXml, typeof(AckRequest)) as AckRequest;
            AckResponse ar = new AckResponse(){ Acknowledged= false };
            if (dbhandler.SessionCheck(aq.Session.Token))
            {
                ar.Acknowledged = dbhandler.AcknowledgeList(aq.Session.Userid);
            }
            return GetXMLString(ar);
        }

        private string GetCalls(string xml)
        {
            ListRequest listReq = GetObjectfromXML(xml, typeof(ListRequest)) as ListRequest;
            CallListXML clx = new CallListXML();
            //check session
            if(dbhandler.SessionCheck(listReq.Session.Token))
            {
                clx = dbhandler.GetCallList(listReq.Args.Campaign, listReq.Session.Userid);
                clx.Session = new session() { Userid = listReq.Session.Userid, Token = listReq.Session.Token};
            }

            return GetXMLString(clx);
        }

        private string DoLogin(string xml)
        {
            LoginXML logins = GetObjectfromXML(xml, typeof(LoginXML)) as LoginXML;
            LoginResponse lr = dbhandler.Login(logins.Args.Username,logins.Args.Password);
            return GetXMLString(lr);
        }

        private string GetXMLString<T>(T arg)
        {
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.NewLineHandling = NewLineHandling.None;
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            sw.NewLine = "";
            XmlWriter xw = XmlWriter.Create(sw,settings);
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(arg.GetType());
            System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            ns.Add("", "");
            xs.Serialize(xw, arg, ns);
            return sw.ToString();
        }

        private Object GetObjectfromXML(string xmlString, Type t)
        {
            using (StreamWriter swr = File.AppendText(@"xmler.txt"))
            {
                swr.WriteLine(xmlString);
            }
            StringReader sw = new StringReader(xmlString);
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(t);
            return xs.Deserialize(sw);
        }
    }
}
