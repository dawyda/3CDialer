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
        List<String> tokens;

        public DBHandler()
        {
            tokens = new List<string>();
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

        internal LoginResponse Login(Logins logins)
        {
            string query = "SELECT u.id,u.name, c.name AS campaign,c.script FROM users u "+
                "INNER JOIN campaigns c  ON c.id = u.campaignid "+ 
                "WHERE u.username = '"+ logins.username +"' AND u.password = '"+ logins.password +"' LIMIT 0,1;";
            LoginResponse lr = new LoginResponse();
            lr.Method = "LoginResponse";
            MySqlCommand cmd = new MySqlCommand(query);
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
                AddSession(lr.args.Userid);
            }
            else
            {
                lr.args.Response = false;
            }
            
            return lr;
        }

        private void AddSession(string p)
        {
            throw new NotImplementedException();
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
    }
    public class Logins
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
