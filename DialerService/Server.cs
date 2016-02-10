using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Dialer;

namespace DialerService
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 8192;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

    class Server
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static MethodHandler methods = new MethodHandler();
        Socket listener;
        IPEndPoint localEndPoint;
        public Server() 
        {
            Settings settings = null;
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(@"C:\ProgramData\3CDialer\settings.xml");
                System.Xml.Serialization.XmlSerializer xmsr = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
                settings = (Settings)xmsr.Deserialize(sr);
                sr.Close();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            localEndPoint = new IPEndPoint(IPAddress.Parse(settings.Service.bindip), Convert.ToInt32(settings.Service.bindport));
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListening()
        {
            byte[] bytes = new Byte[8192];

            //IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 11000);
            //Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(5);
                while (true)
                {
                    allDone.Reset();
                    Logger.LogSocket(DateTime.Now.ToShortDateString() + " inside start listening.");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Logger.LogSocket(DateTime.Now.ToShortDateString() + " socket creation failed/binding failed. " + e.Message);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject so = new StateObject();
            so.workSocket = handler;
            handler.BeginReceive(so.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), so);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            string content = string.Empty;
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));
                content = state.sb.ToString();
                if(content.IndexOf("<br>") > -1){
                    //xml string sent read to end.
                    string resp = methods.Execute(content.Substring(0, (content.Length - 4)));
                    Thread.Sleep(5);
                    Send(handler,resp);
                }
                else{
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, string content)
        {
            if (content == null || content == " " || content == string.Empty)
            {
                content = "<error msg=\"An error occured:" + methods.methodError + "\"/>";
            }
            byte[] byteData = Encoding.ASCII.GetBytes(content);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                Logger.LogSocket(String.Format("Sent {0} bytes to client at {1}.",bytesSent,((IPEndPoint)handler.RemoteEndPoint).Address));
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Logger.LogSocket(DateTime.Now.ToShortDateString() + ": " + e.Message);
            }
        }

        internal void StopListening()
        {
            listener.Close();
            Logger.LogSocket(DateTime.Now.ToShortDateString() + ": socket shutdown.");
        }
    }
}
