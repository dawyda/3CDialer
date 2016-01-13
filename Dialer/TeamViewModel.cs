using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Dialer
{
    public class TeamViewModel: ViewModelEntity
    {
        public TeamViewModel() { }

        protected ObservableCollection<Team> teams = new ObservableCollection<Team>();
        protected Team selectedTeam = null;

        public ObservableCollection<Team> Teams
        {
            get { return teams; }
            set { teams = value; }
        }
        public Team SelectedTeam
        {
            get { return selectedTeam; }
            set
            {
                if (selectedTeam != value)
                {
                    selectedTeam = value;
                    NotifyPropertyChanged("SelectedTeam");
                }
            }
        }
    }
}
