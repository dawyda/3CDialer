using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dialer
{
    public class User : ViewModelEntity
    {
        protected string id;
        protected string name;
        protected string username;
        protected string password;
        protected string role;
        protected string campaign;
        internal string PrevName;

        public User(string name, string username, string password, string role, string campaign,string id)
        {
            this.name = name;
            this.username = username;
            this.password = password;
            this.role = role;
            this.campaign = campaign;
            this.id = id;
            this.PrevName = this.name;
        }

        public User(string name)
        {
            this.name = name;
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
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
        public string Username
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    NotifyPropertyChanged("Username");
                }
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }
        public string Role
        {
            get { return role; }
            set
            {
                if (role != value)
                {
                    role = value;
                    NotifyPropertyChanged("Role");
                }
            }
        }
        public string Campaign
        {
            get { return campaign; }
            set
            {
                if (campaign != value)
                {
                    campaign = value;
                    NotifyPropertyChanged("Campaign");
                }
            }
        }
    }
}
