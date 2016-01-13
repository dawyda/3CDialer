using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dialer
{
    public class Campaign:ViewModelEntity
    {
        protected int id;
        protected string name;
        protected string descr;
        protected int teamId;

        public Campaign(int id, string name, string descr, int teamId) 
        {
            this.id = id;
            this.name = name;
            this.descr = descr;
            this.teamId = teamId;
        }

        public int Id 
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
        public int TeamID
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
    }
}
