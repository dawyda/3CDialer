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

            string connectionString = "SERVER=" + settings.DBserver.IP.Value +
               ";PORT=" + settings.DBserver.IP.port +
               ";DATABASE=" + settings.DBserver.dbname +
               ";UID=" + settings.DBserver.user +
               ";PASSWORD=" + settings.DBserver.password + ";Allow User Variables=false;";
            //using(StreamWriter swr  = File.AppendText(@"gutz.txt"))
            //{
            //    swr.WriteLine(connectionString);
            //}

            try
            {
                conn = new MySqlConnection(connectionString);
            }
            catch (Exception e)
            {
                Logger.LogServiceError(e.Message + ": Could not connect to DB " + " @:" + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        internal bool AcknowledgeList(string userid)
        {
            if (Open())
            {
                var pr = (from pending in receiptsPending
                                    where
                                        pending.userid == userid
                                    select pending).ToList();
                PendingReceipt prs = pr[0];
                string query = "";
                foreach (int id in prs.IDs)
                {
                    query += "UPDATE call_list_data SET assigned = 1 WHERE id = " + id +";";
                }
                int i = 0;
                if((i = new MySqlCommand(query, conn).ExecuteNonQuery()) > 0)
                {
                    receiptsPending.Remove(prs);
                    Close();
                    return true;
                }
            }
           
            Close();
            return false;
        }

        internal LoginResponse Login(string username, string password)
        {
            string query = "SELECT u.id,u.name, c.name AS campaign,c.script FROM users u "+
                "INNER JOIN campaigns c  ON c.id = u.campaignid "+ 
                "WHERE u.username = '"+ username +"' AND u.password = '"+ password +"' LIMIT 0,1;";

            LoginResponse lr = new LoginResponse();
            lr.Method = "LoginResponse";
            RespArgs args = new RespArgs();
            if (Open())
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    //login success
                    args.Token = Hash(DateTime.Now.ToLongTimeString());
                    args.Response = true;
                    //lr.args = new RespArgs() { Response = true,s};
                    //lr.args.Token = Hash(DateTime.Now.ToLongTimeString());
                    tokens.Add(args.Token);
                    while (reader.Read())
                    {
                        args.Userid = reader["id"].ToString();
                        args.Name = reader["name"].ToString();
                        args.Campaign = reader["campaign"].ToString();
                        args.Script = reader["script"].ToString();
                    }
                    reader.Close();
                    if (!AddSession(args.Userid))
                    {
                        ClearSessionError(args.Userid);
                        AddSession(args.Userid);
                    }
                }
                else
                {
                    args.Response = false;
                }
            }
            lr.args = args;

            Close();
            return lr;
        }

        internal CallListXML GetCallList(string campaign, string userid)
        {
            CallListXML clx = new CallListXML();
            double callsinlist = 1;
            double users = 1;
            double calls_per_user = 1;
            string query = "Select ( select count(cd.id) from call_list_data as cd " +
"INNER JOIN call_lists as cl ON cd.calllistID = cl.id "+
"INNER JOIN campaigns as c ON c.id = cl.campaignID "+
"WHERE c.name = '"+ campaign +"' AND cd.status = 'NEW') as calls, "+
"( select count(u.id) FROM users as u "+
"INNER JOIN campaigns as c ON u.campaignId = c.id) as users;";
            if(Open()){
                MySqlDataReader reader = new MySqlCommand(query,conn).ExecuteReader();
                while (reader.Read())
                {
                    callsinlist = Convert.ToDouble(reader["calls"]);
                    users = Convert.ToDouble(reader["users"]);
                }
                List<int> ids = new List<int>();
                List<Call> calls = new List<Call>();
                if (callsinlist > 0)
                {
                    calls_per_user = Math.Ceiling(callsinlist / users);

                    if (calls_per_user > 0)
                    {
                        query = "SELECT c.id, CONCAT(c.fname,' ',c.lname) as name, c.tel1,c.tel2,c.language,c.country,c.custom1,c.custom2,c.custom3," +
                            "c.custom4,c.custom5,c.custom6,c.custom7 from call_list_data as c " +
                            "INNER JOIN call_lists as cl ON c.calllistID = cl.id " +
                            "INNER JOIN campaigns as cc ON cc.id = cl.campaignID " +
                            "WHERE cc.name = '" + campaign + "' AND c.status = 'NEW' AND c.assigned = 0 ORDER BY c.id ASC LIMIT 0," + calls_per_user + ";";
                        reader.Close();
                        reader = new MySqlCommand(query, conn).ExecuteReader();

                        while (reader.Read())
                        {
                            calls.Add(new Call()
                            {
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
                }
                else
                {
                    //no calls in list;
                }
                PendingReceipt p = new PendingReceipt() { userid = userid, IDs = ids };
                receiptsPending.Add(p);
                clx.Args = new CallArgs() { Calls = calls.ToArray()};
            }
            clx.Method = "SetCallList";
            Close();
            return clx;
        }

        private bool AddSession(string id)
        {
            MySqlCommand cmd = new MySqlCommand("INSERT into sessions(userid) VALUES(" + id + ");",conn);
            int res = 0;
            try
            {
                res = cmd.ExecuteNonQuery();
            }
            catch (MySqlException me)
            {
                if (me.Number == 1062)
                {
                    Logger.LogDBError(DateTime.Now.ToLongTimeString() + ": Duplicate session for userid " + id);
                }
            }
            if (!( res> 0))
            {
                Logger.LogDBError("Failed to add user session ? opened earlier.");
                return false;
            }
            return true;
        }

        private void ClearSessionError(string id)
        {
            MySqlCommand cmd = new MySqlCommand("DELETE FROM sessions WHERE userid = "+ id + ";", conn);
            int res = 0;
            try
            {
                res = cmd.ExecuteNonQuery();
            }
            catch (MySqlException me)
            {
                Logger.LogDBError(DateTime.Now.ToLongTimeString() + ": " +me.Message);
            }
            if (!(res > 0))
            {
                Logger.LogDBError("Failed to clear session opened earlier.");
            }
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

        internal bool Logout(session session)
        {
            //remove userid and token
            if (Open())
            {
                tokens.Remove(session.Token);
                string query = "DELETE FROM sessions WHERE userid = " + session.Userid + ";";
                int res = new MySqlCommand(query, conn).ExecuteNonQuery();
                Close();
                return res > 0;
            }
            else
            {
                return false;
            }
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
