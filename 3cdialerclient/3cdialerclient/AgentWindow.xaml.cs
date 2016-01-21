﻿using System;
using System.Collections.Generic;
using System.Text;
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
		public AgentWindow()
		{
			this.InitializeComponent();
			AddKeyShortcuts();
			// Insert code required on object creation below this point.
		}
		protected void AddKeyShortcuts()
		{
			lv_shortList.Items.Add(new {KShort = "Space bar", Function = "End call"});
			lv_shortList.Items.Add(new {KShort = "CTRL + Y", Function = "Redial Last Call"});
			lv_shortList.Items.Add(new {KShort = "F6", Function = "Skip to Next Call"});
			lv_shortList.Items.Add(new {KShort = "Space bar", Function = "End call"});
			lv_shortList.Items.Add(new {KShort = "Space bar", Function = "End call"});
		}
	}
}