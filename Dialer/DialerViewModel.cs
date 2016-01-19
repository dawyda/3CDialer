using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Dialer
{
    public class DialerViewModel: ViewModelEntity
    {
        public DialerViewModel() { }

        //teams
        protected ObservableCollection<Team> teams = new ObservableCollection<Team>();
        protected Team selectedTeam = null;
        //campaigns
        protected ObservableCollection<Campaign> campaigns = new ObservableCollection<Campaign>();
        protected Campaign selectedCampaign = null;
        //users
        protected ObservableCollection<User> users = new ObservableCollection<User>();
        protected User selectedUser = null;
        //settings
        protected SettingsController settingsctrl = new SettingsController();

        public SettingsController SettingsCtrl
        {
            get { return settingsctrl; }
            set
            {
                if (settingsctrl != value)
                {
                    settingsctrl = value;
                    NotifyPropertyChanged("SettingsCtrl");
                }
            }
        }

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

        public ObservableCollection<Campaign> Campaigns
        {
            get { return campaigns; }
            set { campaigns = value; }
        }
        public Campaign SelectedCampaign
        {
            get { return selectedCampaign; }
            set
            {
                if (selectedCampaign != value)
                {
                    selectedCampaign = value;
                    NotifyPropertyChanged("SelectedCampaign");
                }
            }
        }

        public ObservableCollection<User> Users
        {
            get { return users; }
            set { users = value; }
        }
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                if (selectedUser!= value)
                {
                    selectedUser = value;
                    NotifyPropertyChanged("Selecteduser");
                }
            }
        }
    }
}
