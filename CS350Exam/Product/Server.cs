using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data;

namespace CS350Exam.Product
{
    public class Server
    {
        public static OdbcConnection conn = new OdbcConnection("Driver={MySQL ODBC 5.3 UNICODE Driver};" +
                                                               "Server=dragonfirecomputing.com;" +
                                                               "Database=eragon57_cs350;" +
                                                               "User=eragon57_readdb;" +
                                                               "Password=Ce2GoMCdneDEQGAv5dKVQl95XiTHD0QM;" +
                                                               "OPTION=3");

        public static List<User> GetAllUsers()
        {
            conn.Open();
            List<User> users = new List<User>();

            using (conn)
            {
                OdbcCommand cmd = new OdbcCommand("SELECT * from user", conn);
                using (OdbcDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User()
                        {
                            userID = reader.GetString(reader.GetOrdinal("userID")),

                            passSalt = reader.GetString(reader.GetOrdinal("passSalt")),

                            passHash = reader.GetString(reader.GetOrdinal("passHash")),

                            posts = reader.GetString(reader.GetOrdinal("posts")).Length > 0 ?
                                    reader.GetString(reader.GetOrdinal("posts")).Split(',').Select(i => int.Parse(i)).ToList().Count > 0 ?
                                        reader.GetString(reader.GetOrdinal("posts")).Split(',').Select(i => int.Parse(i)).ToList() :
                                        new List<int>() { Convert.ToInt32(reader.GetString(reader.GetOrdinal("posts"))) } :
                                    new List<int>() { },
                                

                            friends = reader.GetString(reader.GetOrdinal("friends")).Length > 0 ?
                                      reader.GetString(reader.GetOrdinal("friends")).Split(',').Select(i => Convert.ToString(i)).ToList().Count > 0 ?
                                        reader.GetString(reader.GetOrdinal("friends")).Split(',').Select(i => Convert.ToString(i)).ToList() :
                                        new List<string>() { reader.GetString(reader.GetOrdinal("friends")) } :
                                      new List<string>() { }
                        });
                    }
                }
            }
            conn.Close();
            return users;
        }

        public static void WriteAllUsers(List<User> users)
        {
            conn.ConnectionString = "Driver={MySQL ODBC 5.3 UNICODE Driver};" +
                                    "Server=dragonfirecomputing.com;" +
                                    "Database=eragon57_cs350;" +
                                    "User=eragon57_readdb;" +
                                    "Password=Ce2GoMCdneDEQGAv5dKVQl95XiTHD0QM;" +
                                    "OPTION=3";
            conn.Open();
            using (conn)
            {
                foreach (User user in users)
                {
                    using (OdbcCommand cmd = new OdbcCommand("INSERT INTO user (userID, passSalt, passHash, posts, friends) VALUES (\"" +
                                                              user.userID + "\",\"" + user.passSalt + "\",\"" + user.passHash + "\",\"" + string.Join(",", user.posts) + "\",\"" + string.Join(",", user.friends) + "\") " + 
                                                              "ON DUPLICATE KEY UPDATE posts = \"" + string.Join(",", user.posts) + "\", friends = \"" + string.Join(",", user.friends) + "\""))
                    {
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            conn.Close();
        }

        public static void DeleteUser(User user)
        {
            conn.Open();

            using (conn)
            {
                using (OdbcCommand cmd = new OdbcCommand("DELETE FROM user WHERE userID = \"" + user.userID + "\""))
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
            }

            conn.Close();
        }

        public static void ResetData()
        {
            conn.Open();

            using (conn)
            {
                using (OdbcCommand cmd = new OdbcCommand("DELETE FROM user"))
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
                using (OdbcCommand cmd = new OdbcCommand("DELETE FROM post"))
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
