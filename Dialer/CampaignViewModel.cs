using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Dialer
{
    public class CampaignViewModel: ViewModelEntity
    {
        public CampaignViewModel() { }

        protected ObservableCollection<Campaign> campaigns = new ObservableCollection<Campaign>();
        protected Team selectedCampaign = null;

        public ObservableCollection<Campaign> Campaigns
        {
            get { return campaigns; }
            set { campaigns = value; }
        }
        public Team SelectedCampaign
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
    }
}
