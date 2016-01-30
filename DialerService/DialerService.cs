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
        Server server;
        public DialerService()
        {
            InitializeComponent();
            server = new Server();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Log(string.Format("Dialer service started at: {0}",DateTime.Now.ToShortTimeString()));
            server.StartListening();
        }

        protected override void OnStop()
        {
            Logger.Log(string.Format("Dialer service stopped at: {0}", DateTime.Now.ToShortTimeString()));
            server.StopListening();
        }

        protected override void OnShutdown()
        {
            Logger.Log(string.Format("Dialer service shutdown at: {0}", DateTime.Now.ToShortTimeString()));
            server.StopListening();   
        }
    }
}
