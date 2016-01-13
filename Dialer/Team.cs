using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dialer
{
    public class Team : ViewModelEntity
    {
        protected string id;
        protected string name;
        protected string descr;

        public Team(string id, string name, string descr) 
        {
            this.id = id;
            this.name = name;
            this.descr = descr;
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
    }
}
