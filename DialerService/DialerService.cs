using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Dialer;

namespace DialerService
{
    public partial class DialerService : ServiceBase
    {
        public DialerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Server server = new Server();
                server.StartListening();
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Dialer service Exception occured at {0}: {1}", DateTime.Now.ToShortTimeString(),e.Message + " \nMore:" + e.InnerException));
            }
            Logger.Log(string.Format("Dialer service started at: {0}",DateTime.Now.ToShortTimeString()));
        }

        protected override void OnStop()
        {
            Logger.Log(string.Format("Dialer service stopped at: {0}", DateTime.Now.ToShortTimeString()));
            //server.StopListening();
        }

        protected override void OnShutdown()
        {
            Logger.Log(string.Format("Dialer service shutdown at: {0}", DateTime.Now.ToShortTimeString()));
            //server.StopListening();   
        }
    }
}
