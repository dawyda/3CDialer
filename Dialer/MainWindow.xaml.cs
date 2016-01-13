using System;
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

        public MainWindow()
        {
            InitializeComponent();
            database  = new DBHandler();
            dgArrangementValues();
            import_campaign.Items.Add("Select Campaign for List");
            foreach(var item in database.GetCampaignsAsList())
            {
                import_campaign.Items.Add(item);
            }
            import_campaign.SelectedIndex = 0;
            getListsForListDataTab();
            initializeCampaignsTab();
            initTeamsTab();
        }

        private void initializeCampaignsTab()
        {
            //set the combo box values
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
            win_mgt.Title = "3CDialer - Management Console - " + ti.Header;
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
            DataTable ldt = database.getCallListbyListID(ListID);
            dg_listData.DataContext = ldt;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //refresh stuff
            getListsForListDataTab();
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
            //get lists from DB
            foreach (Team team in database.GetAllTeams())
            {
                TeamViewModel.Teams.Add(team);
            }
            TeamViewModel.SelectedTeam = TeamViewModel.Teams[0];
        }

        //add team clicked event handler
        private void TeamAdd_Click(object sender, RoutedEventArgs e)
        {
            Team team = new Team("", "New Team", "");
            TeamViewModel.Teams.Add(team);
            TeamViewModel.SelectedTeam = team;
            txt_teamName.SelectAll();
            txt_teamName.Focus();
            addingNew = true;
        }

        private void RefreshAllTabs()
        {
            initializeCampaignsTab();
            initTeamsTab();
        }

        private void Btn_UpdateTeam_Click(object sender, RoutedEventArgs e)
        {
            Team team = TeamViewModel.SelectedTeam;
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
            }
        }

        private void BtnDelTeam_Click(object sender, RoutedEventArgs e)
        {
            Team team = TeamViewModel.SelectedTeam;
            //Don't delete default team.
            if (team.Name == "Default Team")
            {
                MessageBox.Show("Cannot delete default team", "Team Delete", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (database.DeleteTeamByName(team.Name))
            {
                TeamViewModel.SelectedTeam = TeamViewModel.Teams[0];
                TeamViewModel.Teams.Remove(team);
            }
            else
            {
                MessageBox.Show("Failed");
                Logger.LogError("Team deletion failed");
            }
        }
    }
}
