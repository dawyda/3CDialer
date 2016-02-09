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
        int CALL_STATUS_INTERVAL = 2; //every one second.
        int CALL_LIST_REFRESH = 600;// every 5 minues check for changes in call list.
        int UPDATE_USER_STATUS = 1200; //check if user has had his campaign changed.
        Timer wrapuptimer;

        private string prevStatus = "starting";
              
		public AgentWindow(ServerHandler serverHandler)
		{
			this.InitializeComponent();
			AddKeyShortcuts();
			// Insert code required on object creation below this point.
            timer = new Timer(500);
            timer.Elapsed += new ElapsedEventHandler(UpdatesLoop);
            this.serverHandler = serverHandler;
            wrapuptimer = new Timer(1000 * serverHandler.wrapup);
            wrapuptimer.Elapsed += new ElapsedEventHandler(startCall);

            if (listRequested = serverHandler.RequestCallList())
            {
                SetDialCard();
            }
            else
            {
                //no call in list found;
                lv_dialcard.ItemsSource = DialCard.CreateBlank();
                MessageBox.Show("No calls to dial! Ask admin to upload.");
                tb_status.Text = "Status: You have no calls to dial.";
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
            int numCalls = serverHandler.calls == null ? 0 : serverHandler.calls.Count;
            tb_campaign.Text = "Campaign - " + serverHandler.userCampaign + "            ( " + numCalls + " Calls Loaded. )";
            txt_script.Text = serverHandler.campaign_script;
            tb_user.Text = "Logged in as: " + serverHandler.username;
            tb_status.Text = "Client started. Click 'Start calls' button to start dialing.";
		}

        //populate dialcard with details of next call;
        private void SetDialCard()
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    if (!serverHandler.endReached)
                    {
                        try
                        {
                            List<DialCardItem> listdc = DialCard.Create(serverHandler.CurrentCall);
                            if (lv_dialcard.HasItems) lv_dialcard.ItemsSource = null;
                            lv_dialcard.ItemsSource = listdc;
                            txt_notes.Text = "Enter notes about the call here...";
                        }
                        catch (Exception e)
                        {
                            Logger.Log("During set dial card: " + e.Message);
                        }
                        //set other stuff.
                    }
                    else
                    {
                        MessageBox.Show("Current List Calls Completed.\nChecking for more calls. Click OK.", "Well Done!");
                    }
                }));
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
            if (!listRequested)
            {
                MessageBox.Show("No calls to Dial. \nTry to refresh.","Dial Error");
                e.Handled = true;
                return;
            }
            dial = true;
            btn_Stopcalls.IsEnabled = true;
            btn_Startcalls.IsEnabled = false;
            DialNext();
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
            CALL_STATUS_INTERVAL--;
            CALL_LIST_REFRESH--;
            UPDATE_USER_STATUS--;

            if (CALL_STATUS_INTERVAL == 0)
            {
                if(dial){
                    GetCallStatus();
                }
                CALL_STATUS_INTERVAL = 2;//reset timer
            }

            if (CALL_LIST_REFRESH == 0)
            {
                RefreshCalls();
                CALL_LIST_REFRESH = 600; //reset timer
            }

            if (UPDATE_USER_STATUS == 0)
            {
                RefreshUserStatus();
                UPDATE_USER_STATUS = 1200;//reset timer
            }
        }

        private void GetCallStatus()
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    // your code here.
                    try
                    {
                        string status = serverHandler.SP_GetCallStatus();
                        if (prevStatus != status)
                        {
                            prevStatus = status;
                            tb_status.Text = "Phone status: " + status;
                            if (status == "ended")
                            {
                                serverHandler.MarkDialed(txt_notes.Text);
                                if (serverHandler.endReached)
                                {
                                    MessageBox.Show("Calls completed", "Well done!");
                                    RefreshCalls();
                                    return;
                                }
                                wrapuptimer.Start();
                            }
                            else
                            {
                                //update call status at server. activities ike ringing, dialing, etc.
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e.Message + ": Might occur when softphone is not launched.");
                    }
                }));
        }

        private void startCall(object sender, ElapsedEventArgs e)
        {
            wrapuptimer.Stop();
            if (serverHandler.endReached) return;
            SetDialCard();
            DialNext();
            if (serverHandler.popURL.Value)
            {
                object[] args = {serverHandler.CurrentCall.tel1,serverHandler.CurrentCall.tel2,serverHandler.CurrentCall.name };
                System.Diagnostics.Process.Start(String.Format(serverHandler.popURL.address,args));
            }
        }

        private void DialNext()
        {
            if (dial)
            {
                serverHandler.SP_Call();
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshCalls();
            RefreshUserStatus();
            SetDialCard();
        }

        private void RefreshCalls()
        {
            listRequested = serverHandler.RequestCallList();
        }

        private void RefreshUserStatus()
        {
            //to be done later.
        }

        private void btn_EndCall_Click(object sender, RoutedEventArgs e)
        {
            serverHandler.SP_EndCall();
        }
	}
}