using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _cdialerclient
{
	/// <summary>
	/// Interaction logic for AgentWindow.xaml
	/// </summary>
	public partial class AgentWindow : Window
	{
        public ServerHandler serverHandler;
        bool listRequested = false;
        bool dial = false;
        private bool loggingOut = false;
        Timer timer;
        int CALL_STATUS_INTERVAL = 1; //every one second.
        
		public AgentWindow(ServerHandler serverHandler)
		{
			this.InitializeComponent();
			AddKeyShortcuts();
			// Insert code required on object creation below this point.
            timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(UpdatesLoop);
            this.serverHandler = serverHandler;
            if (listRequested = serverHandler.RequestCallList())
            {
                SetDialCard();
            }
            else
            {
                //no call in list found;
                lv_dialcard.ItemsSource = DialCard.CreateBlank();
                MessageBox.Show("No calls to dial! Ask admin to upload.");
            }
            //this timer does updates to server and also the plugin.
            try
            {
                timer.Start();
            }
            catch (ArgumentOutOfRangeException aoe)
            {
                Logger.Log("Timer exception: " + aoe.Message);
            }
		}

        //populate dialcard with details of next call;
        private void SetDialCard()
        {
            if (!serverHandler.endReached)
            {
                try
                {
                    List<DialCardItem> listdc = DialCard.Create(serverHandler.CurrentCall);
                    lv_dialcard.ItemsSource = listdc;
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                MessageBox.Show("Current List Calls Completed.\nChecking for more calls. Click OK.","Well Done!");
            }
        }

		protected void AddKeyShortcuts()
		{
			lv_shortList.Items.Add(new {KShort = "Space bar", Function = "End call"});
			lv_shortList.Items.Add(new {KShort = "Ctrl + Y", Function = "Redial call. Press after end call."});
			lv_shortList.Items.Add(new {KShort = "F6", Function = "Skip to next Call"});
			lv_shortList.Items.Add(new {KShort = "Space bar", Function = "End call"});
			lv_shortList.Items.Add(new {KShort = "Space bar", Function = "End call"});
		}

        private void menu_exit_Click(object sender, RoutedEventArgs e)
        {
            LogoutAndClose();
        }

        private void StartCalls(object sender, RoutedEventArgs e)
        {
            if (!listRequested) return;
            dial = true;
            btn_Stopcalls.IsEnabled = true;
            btn_Startcalls.IsEnabled = false;
        }

        private void StopCalls(object sender, RoutedEventArgs e)
        {
            dial = false;
            btn_Stopcalls.IsEnabled = false;
            btn_Startcalls.IsEnabled = true;
        }

        private void menu_logout_Click(object sender, RoutedEventArgs e)
        {
            //logout only.
            serverHandler.Logout();
            new MainWindow().Show();
            loggingOut = true;
            this.Close();
        }
        protected void LogoutAndClose()
        {
            if (this.serverHandler.Logout())
            {
                this.Close();
            }
            else
            {
                Logger.Log("Logout failed but will close anyway.");
                this.Close();
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
            timer.Close();
            if (loggingOut)
            {
                return;
            }
            var close = MessageBox.Show("Proceed with close and exit ?", "Exiting...", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (!(close == MessageBoxResult.OK))
            {
                e.Cancel = true;
                return;
            }
        }

        private void UpdatesLoop(object sender, ElapsedEventArgs e)
        {
            //call update functions here!

        }
	}
}