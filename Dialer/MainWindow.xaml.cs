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
        private string csvPath = "";

        public MainWindow()
        {
            InitializeComponent();
            dgArrangementValues();
        }

        private void dgArrangementValues()
        {
            DataTable dgt = new DataTable("arrangementTable");
            DataColumn dc = new DataColumn("fieldName", typeof(string));
            dc.Caption = "Database Field Name";
            dgt.Columns.Add(dc);
            dc = new DataColumn("custlabel", typeof(string));
            dc.Caption = "Database Field Name";
            dc.DefaultValue = string.Empty;
            dgt.Columns.Add(dc);
            dc = new DataColumn("fieldPos", typeof(string));
            dc.Caption = "Field Column Position";
            dc.DefaultValue = "0";
            dgt.Columns.Add(dc);

            string[] labels = { "First Name", "Last Name", "Other Names", "Telephone 1", "Telephone 2", "Language", "Country", "Text 1", "Text 2", "Text 3", "Text 4", "Text 5", "Text 6" };

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
                    //cb_ignoreheaders.Unchecked += ignoreHeaderChanged;
                    //cb_ignoreheaders.Checked += ignoreHeaderChanged;
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
            for (uint i = 1; i < 14; i++)
            {
                DataColumn dc = new DataColumn(i.ToString(), typeof(string));
                dc.Caption = i.ToString();
                dt.Columns.Add(dc);
            }
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
            if (cb_ignoreheaders.IsChecked == true) { j = 1; }
            //add row values;
            for (int i = j; i < csvRows.Count; i++)
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

        private void ignoreHeaderChanged(object sender, RoutedEventArgs e)
        {
            ReadCSVtoTable();
        }
    }
}
