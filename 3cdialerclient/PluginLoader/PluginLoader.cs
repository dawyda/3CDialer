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
        private string PhoneState = null;
        private string status = "Starting"; //call status
        private DateTime DialTime = DateTime.Now; //time when dialing started
        private DateTime ConnectedTime = DateTime.Now; //time when call got connected
        private DateTime EndTime = DateTime.Now; //time when user ended call.
        private string Number = ""; // the phone number primary
        private string AltNumber = ""; //alternate phone number.
        private bool hasAlt = true; //has alternative number
        private string dataToSend = "OK";//response to client socket.
        private string error = "OK";//used to report error;
        private bool was_connected = false; //used to check if call got connected then call alt No if hasAlt is true.

        public PluginLoader(IMyPhoneCallHandler callHandler)
        {
            // TODO: Complete member initialization
            this.callHandler = callHandler;
            callHandler.OnCallStatusChanged += new MyPhonePlugins.CallInfoHandler(callHandler_OnCallStatusChanged);
            callHandler.OnMyPhoneStatusChanged += new MyPhonePlugins.MyPhoneStatusHandler(callHandler_OnMyPhoneCallStatusChanged);
        }

        private void callHandler_OnCallStatusChanged(object sender, CallStatus callInfo)
        {
            if (callInfo.State == CallState.Dialing)
            {
                this.status = "dailing";
                this.DialTime = DateTime.Now;
            }
            else if (callInfo.State == CallState.Connected)
            {
                this.status = "connected";
                this.ConnectedTime = DateTime.Now;
                was_connected = true;
            }
            else if (callInfo.State == CallState.Ended)
            {
                this.EndTime = DateTime.Now;
                //check if number failed (end - dial < 1) && hasAlt = true
                if(!was_connected && hasAlt)
                {
                    error = "Failed. Retrying alternate Number.";
                    MakeCall(AltNumber);
                    hasAlt = false;
                    return;
                }
                this.status = "ended";
            }
            else
            {
                //any other state
                this.status = callInfo.State.ToString();
            }
            WriteTrace(callInfo.State.ToString());
        }

        private void callHandler_OnMyPhoneCallStatusChanged(object sender, MyPhoneStatus st)
        {
            if (!sockInit)
            {
                new Thread(new ThreadStart(CreateServerSocket)).Start();
                sockInit = true;
            }
            this.PhoneState = status.ToString();
            if (st == MyPhoneStatus.NoConnection)
            {
                error = "No connection to PBX. Requires pro license.";
            }
            //WriteTrace(status.ToString());
        }

        private void CreateServerSocket()
        {
            byte[] bytes = new byte[1024];
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15501);
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
                    PerfomAction(num[0]);//func to carry out instructions from client.
                    //Thread.Sleep(50); was meant for serialization.
                    string resp = this.dataToSend;
                    client.Send(Encoding.ASCII.GetBytes(resp));
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
            catch(Exception e)
            {
                WriteTrace(e.Message + " : " + e.InnerException);
            }
        }

        private void PerfomAction(string action)
        {
            switch (action)
            {
                case "D":
                    Drop();
                    break;
                //case "H":
                //    Hold();
                //    break;
                //case "U":
                //    Unhold();
                //    break;
                case "GS"://getstatus
                    GetStatus();
                    WriteTrace(action);
                    break;
                case "GD"://getdialtime
                    GetDialTime();
                    break;
                case "GE"://getendtime
                    GetEndTime();
                    break;
                case "GPS"://getphonestatus...will be used later
                    GetPhoneStatus();
                    break;
                case "E"://get error request
                    GetError();
                    break;
                default:
                    if (action.Substring(0, 1) == "C")
                    {
                        CallStatus c = MakeCall(action.Substring(1));
                        lastcallid = c.CallID;
                        dataToSend = "OK";
                    }
                    else
                    {
                        WriteTrace("wrong call received from dialer app.");
                        error = "Unknown/unsupported request received.";
                    }
                    break;
            }
        }

        private void GetError()
        {
            dataToSend = error;
            error = "OK";
        }

        private void GetStatus()
        {
            dataToSend = status;
        }

        private void GetDialTime()
        {
           dataToSend = this.DialTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
        }

        private void GetEndTime()
        {
            dataToSend = this.DialTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
        }

        private void GetPhoneStatus()
        {
            dataToSend = PhoneState;
        }

        private void Unhold()
        {
            //will be implemented in v14.
        }

        private void Hold()
        {
            
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
            //dest is int the form num:altnum so split first
            if (dest.IndexOf(":") > -1)
            {
                string[] tmp = dest.Split(':');
                Number = tmp[0];
                if ((tmp.Length > 1) && (tmp[1].Length > 0))
                {
                    AltNumber = tmp[1];
                    hasAlt = true;
                }
            }
            else
            {
                Number = dest.Trim();
            }
            return callHandler.MakeCall(Number);
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
