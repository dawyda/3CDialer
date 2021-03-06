﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.ServiceProcess;

namespace Dialer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dt;
        private DataTable dgt;
        private string csvPath = "";
        private DBHandler database;
        private int num_import_records = 0;//number of rows in CSV
        private bool addingNew = false;
        private System.Timers.Timer refreshTimer;
        private bool cancelRefresh = false;
        private ServiceController sc = null;
        string connstr = "";

        public MainWindow(String username)
        {
            InitializeComponent();
            connstr = "SERVER=" + DialerViewModel.SettingsCtrl.Settings.DBserver.IP.Value +
                ";PORT=" + DialerViewModel.SettingsCtrl.Settings.DBserver.IP.port +
                ";DATABASE=" + DialerViewModel.SettingsCtrl.Settings.DBserver.dbname +
                ";UID=" + DialerViewModel.SettingsCtrl.Settings.DBserver.user +
                ";PASSWORD=" + DialerViewModel.SettingsCtrl.Settings.DBserver.password + ";";
            database  = new DBHandler(connstr);
            this.Title = this.Title + "  Logged in as " + username;
            dgArrangementValues();
            import_campaign.Items.Add("Select Campaign for List");
            foreach(var item in database.GetCampaignsAsList())
            {
                import_campaign.Items.Add(item);
            }
            import_campaign.SelectedIndex = 0;
            getListsForListDataTab();
            initCampaignsTab();
            initTeamsTab();
            initUsersTab();
            initSettingsTab();
            initTimer();
        }

        //the timer to refresh GUI elems. 1 minute interval.
        private void initTimer()
        {
            refreshTimer = new System.Timers.Timer(120000);
            refreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimedRefresh);
            //disabled till a suitable implementation is found.
            //refreshTimer.Start();
        }

        private void TimedRefresh(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (cancelRefresh) return;
            RefreshAllTabs();
        }

        //SETTINGS TAB CODE HERE
        private void initSettingsTab()
        {
            IPAddress[] host = Dns.GetHostAddresses(Dns.GetHostName());
            cb_ipaddr.Items.Add("127.0.0.1");
            foreach(IPAddress address in host)
            {
                if(address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) cb_ipaddr.Items.Add(address.ToString());
            }
            cb_ipaddr.Items.Add("0.0.0.0");

            try
            {
                sc = new ServiceController("DialerService");
                if ((sc.Status == ServiceControllerStatus.Stopped) || (sc.Status == ServiceControllerStatus.StopPending))
                {
                    tb_serviceStatus.Text = "The Dialer service is stopped. Click to start.";
                    tb_serviceStatus.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    tb_serviceStatus.Text = "The Dialer service is started. Click to stop.";
                    tb_serviceStatus.Foreground = new SolidColorBrush(Colors.Green);
                }
            }
            catch (Exception e)
            {
                Logger.LogServiceError("Service might not be installed. " + e.Message);
            }
            //set license status.
            LicenseStatus();
        }

        private void LicenseStatus()
        {
            string keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\3CDialer";
            string statText = "Failed to get License Status.";
            string DEMO_FILE_PATH = Environment.CurrentDirectory + @"\dlic.txt";
            try
            {
                //check if demo and if grace period is over - if over software should show popup and limit users to 1.
                if (File.Exists(DEMO_FILE_PATH))
                {
                    DateTime activationDate;
                    DateTime today = DateTime.Now;
                    activationDate = DateTime.Parse(File.ReadAllText(DEMO_FILE_PATH));
                    TimeSpan days = today - activationDate;
                    if (days.Days > 30)
                    {
                        //grace period exceeded.
                        Registry.SetValue(keyName, "NumUsers", 1);
                        MessageBox.Show("Trial period Exceeded! Limited to one user only...","Activate License!");
                    }
                    else if (days.Days > 24)
                    {
                        MessageBox.Show("You have less than " + (30 - days.Days) + " days left before your trial expires!", "Activate License!",MessageBoxButton.OK,MessageBoxImage.Information);
                    }
                }
                statText = Microsoft.Win32.Registry.GetValue(keyName, "Status", "Not Activated!").ToString() + " (" + Microsoft.Win32.Registry.GetValue(keyName, "NumUsers", 5).ToString() + " users)";
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to get license details from registry. Info: " + e.Message + " :" +e.InnerException);
            }
            tb_currentLic.Text = statText;
        }

        //USER TAB CODE HERE
        private void initUsersTab()
        {
            DialerViewModel.Users.Clear();
            List<User> users = database.GetAllUsers();
            if(users.Count < 1) users.Add(new User("No Users. Click Below to Add."));
            foreach (User user in users)
            {
                DialerViewModel.Users.Add(user);
            }
            DialerViewModel.SelectedUser = DialerViewModel.Users[0];
            cb_roles.ItemsSource = database.GetRolesAsString();
            cb_userCampaign.ItemsSource = database.GetCampaignsAsList();
            cb_userCampaign.SelectedIndex = 0;
            //cb_roles.SelectedIndex = 0;
        }

        private void initCampaignsTab()
        {
            DialerViewModel.Campaigns.Clear();
            foreach(Campaign campaign in database.GetAllCampaigns())
            {
                DialerViewModel.Campaigns.Add(campaign);
            }
            DialerViewModel.SelectedCampaign = DialerViewModel.Campaigns[0];
            cb_campaignTeam.ItemsSource = database.GetAllTeamsAsList();
        }

        private void getListsForListDataTab()
        {
            cb_listDataFilter.ItemsSource = database.GetCampaignsAsList();
            dg_listData.AlternatingRowBackground = new SolidColorBrush(Colors.Azure);
            lv_lists.DataContext = database.getAllCallLists();
        }

        private void dgArrangementValues()
        {
            dgt = new DataTable("arrangementTable");
            DataColumn dc = new DataColumn("fieldName", typeof(string));
            dc.Caption = "Database Field Name";
            dgt.Columns.Add(dc);
            dc = new DataColumn("custLabel", typeof(string));
            dc.Caption = "Database Field Name";
            dc.DefaultValue = string.Empty;
            dgt.Columns.Add(dc);
            dc = new DataColumn("fieldPos", typeof(string));
            dc.Caption = "Field Column Position";
            dc.DefaultValue = "0";
            dgt.Columns.Add(dc);

            string[] labels = { "First Name", "Last Name", "Telephone 1", "Telephone 2", "Language", "Country", "Text 1", "Text 2", "Text 3", "Text 4", "Text 5", "Text 6","Text 7" };

            for (int i = 0; i < labels.Length; i++)
            {
                DataRow dr = dgt.NewRow();
                dr[0] = labels[i];
                dgt.Rows.Add(dr);
            }
            dg_arrangement.DataContext = dgt;
        }

        private void onTabFocus(object sender, RoutedEventArgs e)
        {
            TabItem ti = (TabItem)sender;
            win_mgt.Title = "3CDialer - Management Console - " + ti.ToolTip;
        }

        private void importBtn_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            OpenFileDialog fd = new OpenFileDialog();
            fd.DefaultExt = ".csv";
            fd.Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            Nullable<bool> result = fd.ShowDialog();
            if (result == true)
            {
                csvPath = fd.FileName;
                tb_csvfile.Text = csvPath;
                try
                {
                    ReadCSVtoTable();
                    dg_import.DataContext = dt;
                    cb_ignoreheaders.Unchecked += ignoreHeaderChanged;
                    cb_ignoreheaders.Checked += ignoreHeaderChanged;
                    btn_save_import.IsEnabled = true;
                    btn_reset.IsEnabled = true;
                }
                catch (Exception exc)
                {
                    Trace.WriteLine(exc.Message);
                }
            }//end if 
        }

        private void ReadCSVtoTable()
        {
            StreamReader sr = new StreamReader(csvPath);
            //strip headers and define data table
            dt = new DataTable("imported CSV");
            DataColumn dc;
            for (uint i = 1; i < 14; i++)
            {
                dc = new DataColumn(i.ToString(), typeof(string));
                dc.Caption = i.ToString();
                dc.DefaultValue = string.Empty;
                dt.Columns.Add(dc);
            }
            dc = new DataColumn("0", typeof(string));
            dc.Caption = "0";
            dc.DefaultValue = string.Empty;
            dt.Columns.Add(dc);
            List<string> csvRows = new List<string>();
            while (true)
            {
                Thread.Sleep(10);
                string line = sr.ReadLine();
                if (line != null)
                {
                    csvRows.Add(line);
                }
                else
                {
                    break;
                }
            }
            int j = 0;
            num_import_records = csvRows.Count;
            if (cb_ignoreheaders.IsChecked == true) { j = 1; }
            //add row values;
            for (int i = j; i < num_import_records; i++)
            {
                DataRow dr = dt.NewRow();
                string[] sarr = csvRows[i].Split(',');
                int colIndex = 0;
                foreach (string rowVal in sarr)
                {
                    dr[colIndex] = rowVal;
                    colIndex++;
                }
                dt.Rows.Add(dr);
            }
        }

        private void ignoreHeaderChanged(object sender, RoutedEventArgs e)//ignore header checkbox is checked/unchecked
        {
            ReadCSVtoTable();
            dg_import.DataContext = dt;
        }

        //save button has been clicked
        private void ImportSaveClick(object sender, RoutedEventArgs e)
        {
            if(txt_listName.Text == "")
            {
                MessageBox.Show("Please enter the list name", "Import Error", MessageBoxButton.OK);
                e.Handled = true;
                return;
            }
            if (import_campaign.SelectedIndex == 0)
            {
                MessageBox.Show("Select Campaign for List", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                import_campaign.Focus();
                e.Handled = true;
                return;
            }
            DataTable dt = (DataTable)dg_arrangement.DataContext;
            if (dt.Rows[2][2].ToString() == "0")
            {
                MessageBox.Show("Please specify column in list with primary telephone number", "Import Error", MessageBoxButton.OK,MessageBoxImage.Error);
                e.Handled = true;
                dg_arrangement.SelectedIndex = 2;
                return;
            }
            if (dt.Rows[0][2].ToString() == "0")
            {
                MessageBox.Show("Please specify column in list with first name", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                dg_arrangement.SelectedIndex = 0;
                return;
            }
            PostImportData();
        }

        private void PostImportData()
        {
            List<Call> callList = new List<Call>();
            //define mapping from arrangement datagrid
            //get mappings
            string fname_field = dgt.Rows[0][2].ToString();
            string lname_field = dgt.Rows[1][2].ToString();
            string tel1_field = dgt.Rows[2][2].ToString();
            string tel2_field = dgt.Rows[3][2].ToString();
            string lang_field = dgt.Rows[4][2].ToString();
            string country_field = dgt.Rows[5][2].ToString();
            string cust1_field = dgt.Rows[6][2].ToString();
            string cust2_field = dgt.Rows[7][2].ToString();
            string cust3_field = dgt.Rows[8][2].ToString();
            string cust4_field = dgt.Rows[9][2].ToString();
            string cust5_field = dgt.Rows[10][2].ToString();
            string cust6_field = dgt.Rows[11][2].ToString();
            string cust7_field = dgt.Rows[12][2].ToString();
            //assign as per mappings
            DataTable mdt = (DataTable)dg_import.DataContext;
            foreach(DataRow row in mdt.Rows)
            {
                Call call = new Call(){
                    fname = (string)row[fname_field],
                    lname = (string)row[lname_field],
                    tel1 = (string)row[tel1_field],
                    tel2 = (string)row[tel2_field],
                    lang = (string)row[lang_field],
                    country = (string)row[country_field],
                    status = "new",
                    custom1 = (string)row[cust1_field],
                    custom2 = (string)row[cust2_field],
                    custom3 = (string)row[cust3_field],
                    custom4 = (string)row[cust4_field],
                    custom5 = (string)row[cust5_field],
                    custom6 = (string)row[cust6_field],
                    custom7 = (string)row[cust7_field]
                };
                callList.Add(call);
            }
            //post to db;
            if (database.AddList(callList, txt_listName.Text, txt_listDesc.Text, import_campaign.SelectedItem.ToString()))
            {
                MessageBox.Show((num_import_records-1) + " records imported successfully", "Import Success", MessageBoxButton.OK);
                ResetImportListTab();
            }
            else
            {
                Logger.LogError("Import Failed");
                MessageBox.Show((num_import_records-1) + " records failed to import", "Import Failed", MessageBoxButton.OK);
            }
        }

        private void lv_lists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lv_lists.SelectedIndex == -1) return;
            Dictionary<string, int> listDict = (Dictionary<string, int>)lv_lists.DataContext;

            int ListID = listDict.ElementAt(lv_lists.SelectedIndex).Value;
            Trace.WriteLine(ListID);
            DataTable ldt = database.getCallListbyListID(ListID);
            dg_listData.DataContext = ldt;
            //Trace.WriteLine(ldt.Rows[0][2]);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //refresh stuff
            getListsForListDataTab();
            initCampaignsTab();
        }
        
        //when the list data combox box filter by campaigns is clicked
        private void cb_listDataFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<CallList> campaigns = database.getCallListbyCampaignName(cb_listDataFilter.SelectedItem.ToString());
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (CallList c in campaigns)
            {
                dict.Add(c.ListName, c.ListID);
            }
            lv_lists.DataContext = dict;
            foreach (var item in dict)
            {
                Trace.WriteLine(item.Key+", "+item.Value);
            }
        }

        //this function resets the import list tab fields to defaults.
        private void ResetImportListTab()
        {
            txt_listDesc.Text = "";
            txt_listName.Text = "";
            btn_save_import.IsEnabled = false;
            import_campaign.SelectedIndex = 0;
            dgArrangementValues();
            tb_csvfile.Text = "No CSV file currently selected.";
            dg_import.DataContext = null;
            btn_save_import.IsEnabled = false;
            csvPath = "";
            cb_ignoreheaders.Unchecked -= ignoreHeaderChanged;
            cb_ignoreheaders.Checked -= ignoreHeaderChanged;
            cb_ignoreheaders.IsChecked = true;
            num_import_records = 0;
            btn_reset.IsEnabled = false;
        }

        //reset button in import list click event
        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            ResetImportListTab();
        }

        //deletes selected list item
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Dictionary<string, int> listDict = (Dictionary<string, int>)lv_lists.DataContext;
            string ListName = listDict.ElementAt(lv_lists.SelectedIndex).Key;
            var confirm = MessageBox.Show("Are you sure you want to delete the list: " + ListName, "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm.Equals(MessageBoxResult.No))
            {
                return;
            }
            if (database.DeleteCallList(ListName))
            {
                MessageBox.Show("1 list deleted", "List Deleted",MessageBoxButton.OK,MessageBoxImage.Information);
                lv_lists.SelectedIndex = -1;
                getListsForListDataTab();
                dg_listData.DataContext = null;
                return;
            }
            Logger.LogError("Failed to delete list named " + ListName);
            MessageBox.Show("List deletion failed","Error");
        }

        private void initTeamsTab()
        {
            DialerViewModel.Teams.Clear();
            //get lists from DB
            foreach (Team team in database.GetAllTeams())
            {
                DialerViewModel.Teams.Add(team);
            }
            DialerViewModel.SelectedTeam = DialerViewModel.Teams[0];
        }

        //add team clicked event handler
        private void TeamAdd_Click(object sender, RoutedEventArgs e)
        {
            Team team = new Team("", "New Team", "");
            DialerViewModel.Teams.Add(team);
            DialerViewModel.SelectedTeam = team;
            txt_teamName.SelectAll();
            txt_teamName.Focus();
            addingNew = true;
        }

        private void RefreshAllTabs()
        {
            DialerViewModel.Campaigns.Clear();
            DialerViewModel.Teams.Clear();
            initCampaignsTab();
            initTeamsTab();
            initUsersTab();
            initSettingsTab();
        }

        private void Btn_UpdateTeam_Click(object sender, RoutedEventArgs e)
        {
            Team team = DialerViewModel.SelectedTeam;
            if (addingNew)
            {
                if (database.addTeam(team))
                {
                    MessageBox.Show("Team Added", "Teams");
                }
                else
                {
                    Logger.LogError("Team add failed for :" + team.Name);
                    MessageBox.Show("Failed", "Teams");
                }
                addingNew = false;
                initTeamsTab();
            }
            else
            {
                MessageBox.Show("To update a team, delete and add new team instead", "Teams update");
            }
            e.Handled = true;
        }

        private void BtnDelTeam_Click(object sender, RoutedEventArgs e)
        {
            Team team = DialerViewModel.SelectedTeam;
            //Don't delete default team.
            if (team.Name == "Default Team")
            {
                MessageBox.Show("Cannot delete default team", "Team Delete", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (database.DeleteTeamByName(team.Name,team.Id))
            {
                DialerViewModel.SelectedTeam = DialerViewModel.Teams[0];
                DialerViewModel.Teams.Remove(team);
                MessageBox.Show("Deleted");
                initTeamsTab();
            }
            else
            {
                MessageBox.Show("Failed");
                Logger.LogError("Team deletion failed");
            }
        }

        private void BtnAddCampaign_Click(object sender, RoutedEventArgs e)
        {
            Campaign campaign = new Campaign("0","New Campaign","","1","Default Team","Enter script here...");
            DialerViewModel.Campaigns.Add(campaign);
            DialerViewModel.SelectedCampaign = campaign;
            txt_CampaignName.Focus();
            addingNew = true;
        }

        private void Btn_DelCampaign_Click(object sender, RoutedEventArgs e)
        {
            Campaign campaign = DialerViewModel.SelectedCampaign;
            //Don't delete default team.
            //also  later add code for preventing deletion while service is running.
            if (campaign.Name == "Default Campaign")
            {
                MessageBox.Show("Cannot delete default Campaign", "Campaign Delete", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (database.DeleteCampaignByName(campaign.Name))
            {
                DialerViewModel.SelectedCampaign = DialerViewModel.Campaigns[0];
                DialerViewModel.Campaigns.Remove(campaign);
                MessageBox.Show("Deleted");
            }
            else
            {
                MessageBox.Show("Failed");
                Logger.LogError("Campaign deletion failed");
            }
        }

        private void Btn_CampaignUpdate_Click(object sender, RoutedEventArgs e)
        {
            Campaign campaign = DialerViewModel.SelectedCampaign;
            if (cb_campaignTeam.SelectedValue == null)
            {
                MessageBox.Show("Select the team for the campaign.");
                cb_campaignTeam.Focus();
                return;
            }
            string tName = cb_campaignTeam.SelectedValue.ToString();
            if (addingNew)
            {
                if (database.AddCampaign(campaign, tName))
                {
                    MessageBox.Show("Campaign Added", "Campaigns");
                }
                else
                {
                    Logger.LogError("Campaign add failed for :" + campaign.Name);
                    MessageBox.Show("Failed!", "Campaigns Error");
                }
                addingNew = false;
            }
            else
            {
                database.DeleteCampaignByID(DialerViewModel.SelectedCampaign.Id);
                database.AddCampaign(DialerViewModel.SelectedCampaign, tName);
                database.MaintainUserCampaigns(DialerViewModel.SelectedCampaign.Id, database.addedCampaignId);
                MessageBox.Show("Campaign Updated", "Campaigns");
            }
        }

        private void cb_campaignTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DialerViewModel.SelectedCampaign.TeamName = cb_campaignTeam.SelectedValue.ToString();
        }

        private void btnUserAdd_Click(object sender, RoutedEventArgs e)
        {
            User user = new User("New User","newuser" + new Random().Next(10011,11111),"","User","Default Campaign","0");
            DialerViewModel.Users.Add(user);
            DialerViewModel.SelectedUser = user;
            addingNew = true;
        }

        private void btnUserDel_Click(object sender, RoutedEventArgs e)
        {
            if (DialerViewModel.SelectedUser.Name != "Default User")
            {
                var res = MessageBox.Show("Are you sure you want to remove the user?", "Remove user", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    if (database.DeleteUser(DialerViewModel.SelectedUser.Name))
                    {
                        User user = DialerViewModel.SelectedUser;
                        DialerViewModel.SelectedUser = DialerViewModel.Users[0];
                        DialerViewModel.Users.Remove(user);
                        MessageBox.Show("User deleted", "Delete User");
                        return;
                    }
                }
                return;
            }
            MessageBox.Show("Cannot delete default user!","User Delete");
        }

        private void btnUpdateUsers_Click(object sender, RoutedEventArgs e)
        {
            if (addingNew)
            {
                if (cb_roles.SelectedIndex == -1 || cb_userCampaign.SelectedIndex == -1)
                {
                    MessageBox.Show("Select role and campaign","User add error",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }
                if (database.AddUser(DialerViewModel.SelectedUser, cb_userCampaign.SelectedValue.ToString(), cb_roles.SelectedValue.ToString()))
                {
                    MessageBox.Show("User added");
                    addingNew = false;
                    initUsersTab();
                }
                else
                {
                    MessageBox.Show("User add failed!", "User Add", MessageBoxButton.OK, MessageBoxImage.Error);
                    Logger.LogError("User addition failed");
                    return;
                }
            }
            else
            {
                //this means user has just edited an item in users list.
                if (cb_roles.SelectedIndex == -1 || cb_userCampaign.SelectedIndex == -1)
                {
                    cb_userCampaign.SelectedItem = DialerViewModel.SelectedUser.Campaign;
                    cb_roles.SelectedItem = DialerViewModel.SelectedUser.Role;
                }
                database.DeleteUserbyID(DialerViewModel.SelectedUser.Id);
                database.AddUser(DialerViewModel.SelectedUser, cb_userCampaign.SelectedValue.ToString(), cb_roles.SelectedValue.ToString());
                database.maintainUsers(DialerViewModel.SelectedUser.Id);
                MessageBox.Show("User updated");
                initUsersTab();
                return;
            }
        }

        //Apply settings button is clicked
        private void Btn_ApplySettings_Click(object sender, RoutedEventArgs e)
        {
            if (DialerViewModel.SettingsCtrl.SaveSettings())
            {
                MessageBox.Show("Settings updated", "Settings");
            }
            else
            {
                MessageBox.Show("Settings update failed!", "Settings",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            e.Handled = true;
        }

        private void BtnTestConn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	string connstr = "SERVER=" + DialerViewModel.SettingsCtrl.Settings.DBserver.IP.Value +
                ";PORT=" + DialerViewModel.SettingsCtrl.Settings.DBserver.IP.port +
                ";DATABASE=" + DialerViewModel.SettingsCtrl.Settings.DBserver.dbname +
                ";UID=" + DialerViewModel.SettingsCtrl.Settings.DBserver.user +
                ";PASSWORD=" + DialerViewModel.SettingsCtrl.Settings.DBserver.password + ";";
            if (DBHandler.TestConnectionString(connstr))
            {
                MessageBox.Show("Test Sucessfull!","SUCCESS",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Test Failed!", "FAILED", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_license_Click(object sender, RoutedEventArgs e)
        {
            LicenseWindow win = new LicenseWindow();
            win.ShowDialog();
            LicenseStatus();
        }

        private void BtnService_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (sc == null)
            {
                tb_serviceStatus.Text = "Service not found on local computer.";
                return;
            }
            sc.Refresh();
            if (sc.Status.Equals(ServiceControllerStatus.StartPending) || sc.Status.Equals(ServiceControllerStatus.Running))
            {
                try
                {
                    sc.Stop();
                    tb_serviceStatus.Text = "Dialer service is stopped. Click to stop.";
                    tb_serviceStatus.Foreground = new SolidColorBrush(Colors.Red);
                    btn_service.Content = "Start Service";
                    sc.Refresh();
                }
                catch (Exception)
                {
                }
            }
            else
            {
                try
                {
                    sc.Start();
                    tb_serviceStatus.Text = "Dialer service is started. Click to start.";
                    tb_serviceStatus.Foreground = new SolidColorBrush(Colors.Green);
                    btn_service.Content = "Stop Service";
                    sc.Refresh();
                }
                catch (Exception)
                {
                }
            }
        }

        private void onWinClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sc.Close();
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Settings));
            FileStream fs = File.OpenRead(@"C:\ProgramData\3CDialer\default_settings.xml");
            Settings sets = xs.Deserialize(fs) as Settings;
            fs.Close();
            DialerViewModel.SettingsCtrl.Settings = sets;
            DialerViewModel.SettingsCtrl.SaveSettings();
        }

        //admin password change clicked
        private void btn_adminChange_Click(object sender, RoutedEventArgs e)
        {
            AdminPasswordWindow adminwin = new AdminPasswordWindow(connstr);
            adminwin.ShowDialog();
            e.Handled = true;
        }
    }
}
