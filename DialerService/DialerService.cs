using Dialer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace DialerService
{
    public partial class DialerService : ServiceBase
    {
        Thread serverThread;
        public DialerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                serverThread = new Thread(new ThreadStart(new Server().StartListening));
                serverThread.Start();
                this.EventLog.WriteEntry("Socket started with thread id: " + serverThread.ManagedThreadId.ToString());
            }
            catch(Exception e)
            {
                Logger.LogServiceError(e.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                serverThread.Abort();
                this.EventLog.WriteEntry("Socket stopped with thread id: " + serverThread.ManagedThreadId.ToString());
            }
            catch(Exception e)
            {
                this.EventLog.WriteEntry(e.InnerException+ ": " + e.Message+": " + serverThread.ManagedThreadId.ToString(),EventLogEntryType.Error);
                Logger.LogServiceError(e.Message);
            }
        }
    }
}
