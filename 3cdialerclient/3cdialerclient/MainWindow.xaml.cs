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

namespace _cdialerclient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	Window win = new AgentWindow();
			win.Show();
			this.Close();
        }

        private void btnCancelLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	
        }
    }
}
