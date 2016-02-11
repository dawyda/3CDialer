using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading;
using System.Diagnostics;

/**
 *This class will handle the DB stuff and funcs
 *
 * */
namespace Dialer
{
    class DBHandler
    {
        private MySqlConnection conn;
        internal int addedCampaignId;
        public DBHandler(string connectionString)
        {
            try
            {
                conn = new MySqlConnection(connectionString);
            }
            catch (Exception e)
            {
                Logger.LogDBError(e.Message + ": Could not connect to DB ");
            }
        }

        internal static bool TestConnectionString(string str)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection(str);
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                Logger.LogDBError(e.ToString());
                return false;
            }
        }

        private bool OpenConn()
        {
            try
            {
                conn.Open();
                return true;
            }
            catch(MySqlException e)
            {
                Logger.LogDBError(e.Message);
                return false;
            }
        }

        public Dictionary<string, int> getAllCallLists()
        {
            Dictionary<string, int> listDict = new Dictionary<string, int>();
            if (OpenConn() == true)
            {
                string query = "SELECT * FROM call_lists_view;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    listDict.Add(Convert.ToString(reader[0]),Convert.ToInt32(reader[1]));
                }
                conn.Close();
                return listDict;
            }
            else
            {
                Logger.LogError("Failed to fetch lists in list tab.");
                listDict.Add("Error: No lists fetched", -1);
                return listDict;
            }
        }

        public DataTable getCallListbyListID(int listID)
        {
            DataTable dt = new DataTable("listById");
            for (int i = 0; i < 14; i++)
            {
                DataColumn dc = new DataColumn("header" + i, typeof(string));
                dt.Columns.Add(dc);
            }
            string query = "SELECT CONCAT(fname,' ',lname) AS fullname,tel1,tel2,status,language,country,custom1,custom2,custom3,custom4,custom5,custom6,custom7 FROM call_list_data WHERE calllistID = " + listID + " ORDER BY id LIMIT 0, 1000;";
            if(OpenConn() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                
                while(reader.Read())
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = reader[0].ToString();
                    dr[1] = reader[1].ToString();
                    dr[2] = reader[2].ToString();
                    dr[3] = reader[3].ToString();
                    dr[4] = reader[4].ToString();
                    dr[5] = reader[5].ToString();
                    dr[6] = reader[6].ToString();
                    dr[7] = reader[7].ToString();
                    dr[8] = reader[8].ToString();
                    dr[9] = reader[9].ToString();
                    dr[10] = reader[10].ToString();
                    dr[11] = reader[11].ToString();
                    dr[12] = reader[12].ToString();
                    dt.Rows.Add(dr);
                }
            }
            conn.Close();
            return dt;
        }

        public List<CallList> getCallListbyCampaignName(string campaign)
        {
            int campaignID = getCampaignID(campaign);
            Thread.Sleep(10);
            List<CallList> cList = new List<CallList>();
            if (OpenConn() == true)
            {
                string query = "SELECT name, id FROM call_lists WHERE campaignID = " + campaignID + ";";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    CallList c = new CallList();
                    c.ListName = Convert.ToString(reader[0]);
                    c.ListID = Convert.ToInt32(reader[1]);
                    cList.Add(c);
                }
                conn.Close();
                return cList;
            }
            else
            {
                Logger.LogError("Failed to fetch lists in list tab.");
                cList.Add(new CallList(){ListName = "Error: No lists fetched",ListID = -1});
                return cList;
            }
        }

        public bool DeleteCallList(string listName)
        {
            if(OpenConn()==true)
            {
                string query = "DELETE FROM call_lists WHERE name = '" + listName + "';";
                MySqlCommand cmd = new MySqlCommand(query,conn);
                int res = cmd.ExecuteNonQuery();
                if (res != 0)
                {
                    conn.Close();
                    return true;
                }
            }
            return false;
        }

        private string Hash(string input)
        {
            using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public bool AddList(List<Call> list, string name, string desc = "", string campaign = "")
        {
            int campaignID = getCampaignID(campaign);
            if (campaignID == 0)
            {
                Logger.LogDBError("Failed to get campaign ID!!");
                return false;
            }

            if (OpenConn() == true)
            {
                try
                {
                    string query = "INSERT INTO call_lists(name,descr,campaignID) VALUES('" + name + "','" + desc + "'," + campaignID + ");";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    long last_insert_id = cmd.LastInsertedId;
                    query = "";
                    foreach (var call in list)
                    {
                        query += "INSERT INTO call_list_data (calllistID,fname,lname,tel1,tel2,status,language,country,custom1,custom2,custom3,custom4,custom5,custom6,custom7) VALUES (" + last_insert_id + ",'" +
                            call.fname + "','" +
                            call.lname + "','" +
                            call.tel1 + "','" +
                            call.tel2 + "','" +
                            call.status + "','" +
                            call.lang + "','" +
                            call.country + "','" +
                            call.custom1 + "','" +
                            call.custom2 + "','" +
                            call.custom3 + "','" +
                            call.custom4 + "','" +
                            call.custom5 + "','" +
                            call.custom6 + "','" +
                            call.custom7 + "');";
                    }
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    return true;
                }
                catch (Exception e)
                {
                    Logger.LogDBError(e.Message);
                    return false;
                }
            }
            return false;
        }

        private int getCampaignID(string campaign)
        {
            if(OpenConn() == true)
            {
                string query = "SELECT id FROM campaigns WHERE name = '" + campaign + "';";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                object i = 0;
                while (reader.Read())
                {
                    i = reader[0];
                }
                conn.Close();
                return Convert.ToInt32(i);
            }
            return 0;
        }

        public List<string> GetCampaignsAsList()
        {
            List<string> campaignList = new List<string>();
            string query = "SELECT name FROM campaigns ORDER BY id ASC";
            if (OpenConn() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    campaignList.Add(reader["name"].ToString());
                }
                conn.Close();
                return campaignList;
            }
            else
            {
                campaignList.Add("Could not connect to DB");
                return campaignList;
            }
        }

        internal List<Campaign> GetAllCampaigns()
        {
            List<Campaign> campaigns = new List<Campaign>();
            if (OpenConn() == true)
            {
                try
                {
                    string query = "SELECT c.id,c.name,c.descr,c.teamID,c.script,t.name AS teamName FROM campaigns AS c " +
                    "INNER JOIN teams AS t ON c.teamID = t.id LIMIT 0,50;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        campaigns.Add(new Campaign(reader["id"].ToString(), reader["name"].ToString(), reader["descr"].ToString(), reader["teamID"].ToString(), reader["teamName"].ToString(), reader["script"].ToString()));
                    }
                    conn.Close();
                }
                catch (MySqlException e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            else
            {
                campaigns.Add(new Campaign(
                    //{
                            "-1",
                            "DB Load Failed",
                            "This is an error. Restart Application and Services.",
                            "-1",
                            "None",
                            ""
                        ));
                Logger.LogDBError("Could not load teams list from DB ");
            }
            return campaigns;
        }

        internal List<Team> GetAllTeams()
        {
            List<Team> teams = new List<Team>();
            if (OpenConn() == true)
            {
                string query = "SELECT id,name,descr FROM teams ORDER BY name ASC LIMIT 0,40;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    teams.Add(new Team(reader["id"].ToString(),reader["name"].ToString(),reader["descr"].ToString()));
                }
                conn.Close();
            }
            else
            {
                teams.Add(new Team(
                        //{
                            "-1",
                            "DB Load Failed",
                            "This is an error. Restart Application and Services."
                        ));
                Logger.LogDBError("Could not load teams list from DB " + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return teams;
        }

        //team deletion
        internal bool DeleteTeamByName(string teamName,string id)
        {
            if (OpenConn() == true)
            {
                string query = "DELETE FROM TEAMS WHERE name = '"+teamName+"'; UPDATE campaigns SET teamID = 1 WHERE teamID = " + id + ";";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
                catch (MySqlException e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return false;
        }

        //will get another use for it later.
        internal string GetCampaignNameFromID(string id)
        {
            string cname = "Failed to fetch";
            if (OpenConn() == true)
            {
                string query = "SELECT name FROM teams WHERE id="+id+";";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    cname = reader["name"].ToString();
                }
                conn.Close();
                return cname;
            }
            return cname;
        }

        internal bool addTeam(Team team)
        {
            if (OpenConn() == true)
            {
                try
                {
                    string query = "INSERT INTO teams (name,descr) VALUES('" + team.Name + "','" + team.Descr + "');";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
                catch (MySqlException e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
            return false;
        }

        internal System.Collections.IEnumerable GetAllTeamsAsList()
        {
            List<string> teams = new List<string>();
            if (OpenConn() == true)
            {
                MySqlDataReader reader = new MySqlCommand("SELECT name FROM teams;", conn).ExecuteReader();
                while (reader.Read())
                {
                    teams.Add(reader[0].ToString());
                }
                conn.Close();
            }
            else
            {
                teams.Add("Failed to fetch teams see log");
            }
            return teams;
        }

        internal bool DeleteCampaignByName(string campaignName)
        {
            if (OpenConn() == true)
            {
                string query = "DELETE FROM campaigns WHERE name = '" + campaignName + "';";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
                catch (MySqlException e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return false;
        }
        internal bool DeleteCampaignByID(string id)
        {
            if (OpenConn() == true)
            {
                string query = "DELETE FROM campaigns WHERE id = " + id + ";";
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
                catch (MySqlException e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return false;
        }
        internal bool AddCampaign(Campaign campaign, string tName)
        {
            if (OpenConn() == true)
            {
                string query = "INSERT INTO campaigns (name,descr,script,teamID) SELECT '" + campaign.Name + "','"+ campaign.Script +"','" + campaign.Descr + "',id FROM teams WHERE name = '" + tName + "';";
                try
                {
                    Trace.WriteLine(campaign.TeamName);
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    addedCampaignId = (int)cmd.LastInsertedId;
                    conn.Close();
                    return true;
                }
                catch (MySqlException e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return false;
        }

        //users stuff will be put here
        internal List<User> GetAllUsers()
        {
            string query = "SELECT u.id,u.username,u.name,r.name AS role,c.name AS campaign FROM users u " +
                "INNER JOIN roles r ON u.roleID = r.id " +
                "INNER JOIN campaigns c ON u.campaignID = c.id "+
                "WHERE u.id <> 1;";
            //limit to be added based on license.
            List<User> users = new List<User>();
            if (OpenConn() == true)
            {
                try
                {
                    MySqlDataReader reader = new MySqlCommand(query, conn).ExecuteReader();
                    while (reader.Read())
                    {
                        users.Add(new User(reader["name"].ToString(),
                            reader["username"].ToString(),
                            "password is hashed",
                            reader["role"].ToString(),
                            reader["campaign"].ToString(),
                            reader["id"].ToString()
                            ));
                    }
                    conn.Close();
                }
                catch (Exception e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    users.Add(new User("User fetch failed"));
                }
            }
            else
            {
                users.Add(new User("User fetch failed"));
            }
            return users;
        }

        internal List<string> GetRolesAsString()
        {
            List<string> roles = new List<string>();
            if(OpenConn() == true)
            {
                try
                {
                    MySqlDataReader reader = new MySqlCommand("SELECT name FROM roles;", conn).ExecuteReader();
                    while (reader.Read())
                    {
                        roles.Add(reader["name"].ToString());
                    }
                    conn.Close();
                }
                catch (Exception e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    roles.Add("User fetch failed");
                }
            }
            return roles;
        }
        internal bool DeleteUser(string name)
        {
            string query = "DELETE FROM users WHERE name = '" + name + "';";
            bool deleted = false;
            if (OpenConn() == true)
            {
                try
                {
                    new MySqlCommand(query, conn).ExecuteNonQuery();
                    deleted = true;
                    conn.Close();
                }
                catch(MySqlException e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return deleted;
        }
        internal bool DeleteUserbyID(string id)
        {
            string query = "DELETE FROM users WHERE id = " + id + ";";
            bool deleted = false;
            if (OpenConn() == true)
            {
                try
                {
                    new MySqlCommand(query, conn).ExecuteNonQuery();
                    deleted = true;
                    conn.Close();
                }
                catch (MySqlException e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            return deleted;
        }
        internal bool AddUser(User user, string campaign, string role)
        {
            bool added = false;
            user.Campaign = campaign;
            user.Role = role;
            int cid = getCampaignID(user.Campaign);
            if(OpenConn() != added)
            {
                try
                {
                    string query = "INSERT INTO users (username,name,password,campaignID,roleID) SELECT '" + user.Username + "','" + user.Name + "','" + Hash(user.Password) + "'," + cid + ",r.id FROM roles AS r " +
                        "WHERE r.name = '" + user.Role + "';";
                    int ins = new MySqlCommand(query, conn).ExecuteNonQuery();
                    if (ins != 0)
                    {
                        added = true;
                    }
                    else { Logger.LogDBError("User add failed : " + System.Reflection.MethodBase.GetCurrentMethod().Name); }
                }
                catch (MySqlException e)
                {
                    Logger.LogDBError(e.Message + " :" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
            conn.Close();
            return added;
        }

        internal bool MaintainUsers(string prevId, int newId)
        {
            if(OpenConn() || conn.State == ConnectionState.Open)
            {
                string query = "UPDATE users SET campaignID = " + newId + " WHERE campaignID = " + prevId + ";" +
                    "UPDATE call_lists SET campaignID = " + newId + " WHERE campaignID = " + prevId + ";";
                if (new MySqlCommand(query, conn).ExecuteNonQuery() > -1)
                {
                    conn.Close();
                    return true;
                }
            }
            conn.Close();
            return false;
        }
    }
}
