using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyPhonePlugins;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace PluginLoader
{
    [MyPhonePlugins.CRMPluginLoader]
    public class PluginLoader
    {
        private IMyPhoneCallHandler callHandler = null;
        private static PluginLoader instance = null;
        public static string data = null;
        private bool sockInit = false;
        private string lastcallid = null;

        public PluginLoader(IMyPhoneCallHandler callHandler)
        {
            // TODO: Complete member initialization
            this.callHandler = callHandler;
            callHandler.OnCallStatusChanged += new MyPhonePlugins.CallInfoHandler(callHandler_OnCallStatusChanged);
            callHandler.OnMyPhoneStatusChanged += new MyPhonePlugins.MyPhoneStatusHandler(callHandler_OnMyPhoneCallStatusChanged);
        }

        private void callHandler_OnCallStatusChanged(object sender, CallStatus callInfo)
        {
            WriteTrace(callInfo.State.ToString());
        }

        private void callHandler_OnMyPhoneCallStatusChanged(object sender, MyPhoneStatus status)
        {
            if (!sockInit)
            {
                new Thread(new ThreadStart(CreateServerSocket)).Start();
                sockInit = true;
            }
        }

        private void CreateServerSocket()
        {
            byte[] bytes = new byte[1024];
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 15501);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(ep);
                listener.Listen(1);

                while (true)
                {
                    Socket client = listener.Accept();
                    data = null;
                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = client.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<br>") > -1)
                        {
                            break;
                        }
                    }
                    //num received
                    string[] num = data.Split('<');
                    PerfomCall(num[0]);
                    client.Send(Encoding.ASCII.GetBytes("OK"));
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
            catch(Exception e)
            {
                WriteTrace(e.Message);
            }
        }

        private void PerfomCall(string p)
        {
            switch (p)
            {
                case "D":
                    Drop();
                    break;
                case "H":
                    Hold();
                    break;
                case "U":
                    Unhold();
                    break;
                default:
                    CallStatus c = MakeCall(p);
                    //WriteTrace(c.State.ToString());
                    lastcallid = c.CallID;
                    break;
            }
        }

        private void Unhold()
        {
            //will be implemented in v14.
        }

        private void Hold()
        {
            throw new NotImplementedException();
        }

        private void WriteTrace(string s)
        {
            Logger.Log(s);
        }

        [MyPhonePlugins.CRMPluginInitializer]
        public static void Loader(IMyPhoneCallHandler callHandler)
        {
            instance = new PluginLoader(callHandler);
        }

        public MyPhonePlugins.CallStatus MakeCall(string dest)
        {
            return callHandler.MakeCall(dest);
        }

        public void Drop()
        {
            if (lastcallid != null)
            {
                callHandler.DropCall(lastcallid);
                lastcallid = null;
            }
        }
    }
}
