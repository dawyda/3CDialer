using System;
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
using System.ComponentModel;

namespace _cdialerclient
{
	public class ViewModelEntity: INotifyPropertyChanged
	{
		public ViewModelEntity()
		{
			// Insert code required on object creation below this point.
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public virtual void NotifyPropertyChanged(string propertyName)
		{
			if(PropertyChanged != null)
			{
				PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}