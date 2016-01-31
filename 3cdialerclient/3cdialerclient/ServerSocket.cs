using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace _cdialerclient
{
    public class ServerSocket
    {
        public string responseXml;

        public ServerSocket() 
        {
            responseXml = "";
        }
        /**
         * This method will be called iternally from other more specific methods.
         * login calls it directly
         * other funcs will do more before calling it e.g. session setting.
         * */
        private bool SendtoServer(string data)
        {
            bool datasent = false;
            // Data buffer for incoming data.
            byte[] bytes = new byte[4096];
            // Connect to server.
            try
            {
                //check if hostname of ip first; to be done later.
    //            string ip = ClientSettingsHandler.GetSettings().Server.ip;
    //            if(Regex.IsMatch(ip, "(2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?")) {
    //// the string is an IP
                Socket sender = null;
                IPAddress ipAddress = null;
                IPEndPoint remoteEP = null;
                try
                {
                    ipAddress = IPAddress.Parse(ClientSettingsHandler.GetSettings().Server.ip);
                    remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32(ClientSettingsHandler.GetSettings().Server.port));

                    sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message + ": " + e.InnerException);
                }

                // Connect the socket to the remote endpoint. Catch any errors.
                //this could be moved in the background later via a thread.
                try
                {
                    sender.Connect(remoteEP);

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes(data + "<br>");

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the server.
                    int bytesRec = sender.Receive(bytes);

                    responseXml = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    return datasent = true;
                }
                catch (ArgumentNullException ane)
                {
                    Logger.Log(ane.Message + ": " + ane.InnerException);
                }
                catch (SocketException se)
                {
                    Logger.Log(se.Message + ": " + se.InnerException);
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message + ": " + e.InnerException);
                }

            }
            catch (Exception e)
            {
                Logger.Log(e.Message + ": " + e.InnerException);
            }
            return datasent;
        }
        //does login and sets session
        internal bool GET(string loginString)
        {
            if (SendtoServer(loginString))
            {
                return true;
            }
            return false;
        }
    }
}
