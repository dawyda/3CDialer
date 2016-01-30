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
            localEndPoint = new IPEndPoint(IPAddress.Any, 11000);
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
                listener.Listen(50);
                while (true)
                {
                    allDone.Reset();
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
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
                    string resp = methods.Execute(content);
                    Thread.Sleep(20);
                    Send(handler,resp);
                }
                else{
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, string content)
        {
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
                Console.WriteLine(e.ToString());
            }
        }

        internal void StopListening()
        {
            listener.Close();
        }
    }
}
