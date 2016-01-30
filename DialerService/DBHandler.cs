using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Dialer;
using System.Xml.Serialization;
using System.IO;
using _cdialerclient;

namespace DialerService
{
    class DBHandler
    {
        MySqlConnection conn;
        Settings settings;
        public List<String> tokens;
        public List<PendingReceipt> receiptsPending;

        public DBHandler()
        {
            tokens = new List<string>();
            receiptsPending = new List<PendingReceipt>();
            try
            {
                StreamReader sr = new StreamReader(@"C:\ProgramData\3CDialer\settings.xml");
                XmlSerializer xmsr = new XmlSerializer(typeof(Settings));
                settings = (Settings)xmsr.Deserialize(sr);
                sr.Close();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message + ": " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            string connectionString = "SERVER=" + settings.DBserver.IP +
               ";PORT=" + settings.DBserver.IP.port +
               ";DATABASE=" + settings.DBserver.dbname +
               ";UID=" + settings.DBserver.user +
               ";PASSWORD=" + settings.DBserver.password + ";";

            try
            {
                conn = new MySqlConnection(connectionString);
            }
            catch (Exception e)
            {
                Logger.LogServiceError(e.Message + ": Could not connect to DB " + " @:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        internal LoginResponse Login(string username, string password)
        {
            string query = "SELECT u.id,u.name, c.name AS campaign,c.script FROM users u "+
                "INNER JOIN campaigns c  ON c.id = u.campaignid "+ 
                "WHERE u.username = '"+ username +"' AND u.password = '"+ password +"' LIMIT 0,1;";

            LoginResponse lr = new LoginResponse();
            lr.Method = "LoginResponse";
            if (Open())
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    //login success
                    lr.args.Response = true;
                    lr.args.Token = Hash(DateTime.Now.ToLongTimeString());
                    tokens.Add(lr.args.Token);
                    while (reader.Read())
                    {
                        lr.args.Userid = reader["id"].ToString();
                        lr.args.Name = reader["name"].ToString();
                        lr.args.Campaign = reader["campaign"].ToString();
                        lr.args.Script = reader["script"].ToString();
                    }
                    if (!AddSession(lr.args.Userid))
                    {
                        //add error tag in response xml. User will logou then log in again.
                    }
                }
                else
                {
                    lr.args.Response = false;
                }
            }

            Close();
            return lr;
        }

        internal CallListXML GetCallList(string campaign, string userid)
        {
            CallListXML clx = new CallListXML();
            int calls_per_user = 0;
            string query = "SELECT @calls := (select count(cd.id) from call_list_data as cd " +
                    "INNER JOIN call_lists as cl ON cd.calllistID = cl.id " +
                    "INNER JOIN campaigns as c ON c.id = cl.campaignID " +
                    "WHERE c.name = '" + campaign + "' AND cd.status = 'NEW'); " +
                    "SELECT @users := (select count(u.id) FROM users as u " +
                    "INNER JOIN campaigns as c ON u.campaignId = c.id); " +
                    "SELECT @calls/@users as calls_per_user;";
            if(Open()){
                MySqlDataReader reader = new MySqlCommand(query,conn).ExecuteReader();
                while (reader.Read())
                {
                    calls_per_user = Convert.ToInt32(reader[0]);
                }
                List<int> ids = new List<int>();
                List<Call> calls = new List<Call>();
                if (calls_per_user > 0)
                {
                    query = "SELECT c.id, CONCAT(c.fname,' ',c.lname) as name, c.tel1,c.tel2,c.language,c.country,c.custom1,c.custom2,c.custom3,"+
                        "c.custom4,c.custom5,c.custom6,c.custom7 from call_list_data as c " + 
                        "INNER JOIN call_lists as cl ON c.calllistID = cl.id "+
                        "INNER JOIN campaigns as cc ON cc.id = cl.campaignID "+
                        "WHERE cc.name = '"+ campaign +"' AND c.status = 'NEW' ORDER BY c.id ASC LIMIT 0," + calls_per_user +";";
                    reader = new MySqlCommand(query, conn).ExecuteReader();
                    
                    while(reader.Read())
                    {
                        calls.Add(new Call() { 
                            id = Convert.ToInt32(reader["id"]),
                            name = reader["name"].ToString(),
                            tel1 = reader["tel1"].ToString(),
                            tel2 = reader["tel2"].ToString(),
                            lang = reader["language"].ToString(),
                            country = reader["country"].ToString(),
                            custom1 = reader["custom1"].ToString(),
                            custom2 = reader["custom2"].ToString(),
                            custom3 = reader["custom3"].ToString(),
                            custom4 = reader["custom4"].ToString(),
                            custom5 = reader["custom5"].ToString(),
                            custom6 = reader["custom6"].ToString(),
                            custom7 = reader["custom7"].ToString()
                        });
                        ids.Add(Convert.ToInt32(reader["id"]));
                    }
                }
                PendingReceipt p = new PendingReceipt() { userid = userid,IDs= ids};
                receiptsPending.Add(p);
                clx.Args.Calls = calls.ToArray();
            }
            clx.Method = "SetCallList";
            
            return clx;
        }

        private bool AddSession(string id)
        {
            MySqlCommand cmd = new MySqlCommand("INSERT into sessions(userid) VALUES(" + id + ");",conn);
            int res = cmd.ExecuteNonQuery();
            if (!( res> 0))
            {
                Logger.LogDBError("Failed to add user session ? opened earlier.");
                return false;
            }
            return true;
        }

        internal bool Open()
        {
           
            try
            {
                conn.Open();
                return true;
            }
            catch (MySqlException e)
            {
                Logger.LogServiceError(e.Message + " @" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }

        internal void Close()
        {
            conn.Close();
        }

        private static string Hash(string input)
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
        
        internal bool SessionCheck(string token)
        {
            //check if session exists;
            return (this.tokens.IndexOf(token) > -1);
        }
    }
    //public class Logins
    //{
    //    public string username { get; set; }
    //    public string password { get; set; }
    //}
    class PendingReceipt
    {
        public string userid { get; set; }
        public List<int> IDs { get; set; }
    }
}
