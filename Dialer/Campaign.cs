using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dialer
{
    public class Campaign:ViewModelEntity
    {
        protected string id;
        protected string name;
        protected string descr;
        protected string teamId;
        protected string teamName;

        public Campaign(string id, string name, string descr, string teamId, string teamName) 
        {
            this.id = id;
            this.name = name;
            this.descr = descr;
            this.teamId = teamId;
            this.teamName = teamName;
        }

        public string Id 
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        public string Descr
        {
            get { return descr; }
            set
            {
                if (descr != value)
                {
                    descr = value;
                    NotifyPropertyChanged("Descr");
                }
            }
        }
        public string TeamID
        {
            get { return teamId; }
            set
            {
                if (teamId != value)
                {
                    teamId = value;
                    NotifyPropertyChanged("TeamID");
                }
            }
        }

        public string TeamName
        {
            get { return teamName; }
            set
            {
                if (teamId != value)
                {
                    teamId = value;
                    NotifyPropertyChanged("TeamName");
                }
            }
        }
    }
}
