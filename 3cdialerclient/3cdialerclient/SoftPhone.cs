using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

namespace _cdialerclient
{
    public class SoftPhone
    {
        public string response;
        public bool netError = false;

        public SoftPhone()
        {
            response = "";
        }
        private bool Request(string data)
        {
            bool datasent = false;
            // Data buffer for incoming data.
            byte[] bytes = new byte[4096];
            // Connect to server.
            try
            {
                Socket sender = null;
                try
                {
                    sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    if (netError) netError = false;
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message + ": " + e.InnerException);
                    netError = true;
                }

                // Connect the socket to the remote endpoint. Catch any errors.
                //this could be moved in the background later via a thread.
                try
                {
                    sender.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"),15501));

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes(data + "<br>");

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the server.
                    int bytesRec = sender.Receive(bytes);

                    response = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    if (netError) netError = !netError;
                    return datasent = true;
                }
                catch (ArgumentNullException ane)
                {
                    Logger.Log(ane.Message + ": " + ane.InnerException);
                    netError = true;
                }
                catch (SocketException se)
                {
                    Logger.Log(se.Message + ": " + se.InnerException);
                    netError = true;
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message + ": " + e.InnerException);
                    netError = true;
                }

            }
            catch (Exception e)
            {
                Logger.Log(e.Message + ": " + e.InnerException);
                netError = true;
            }
            return datasent;
        }

        internal bool GET(string sendString)
        {
            bool success = false;
            try
            {
                Thread getThread = new Thread(() => { success = this.Request(sendString); });
                getThread.Start();
                getThread.Join(500);
            }
            catch (Exception e)
            {
                Logger.Log("Error in thread: " + e.Message);
            }
            return success;
        }
    }
}