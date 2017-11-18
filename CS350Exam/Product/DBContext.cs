using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;

namespace CS350Exam.Product
{
    public class DBContext
    {

        private static string connstring = "Driver={MySQL ODBC 5.3 UNICODE Driver};" +
                                           "Server=dragonfirecomputing.com;" +
                                           "Database=eragon57_cs350;" +
                                           "User=eragon57_readdb;" +
                                           "Password=Ce2GoMCdneDEQGAv5dKVQl95XiTHD0QM;" +
                                           "OPTION=3";
        public static OdbcConnection conn;

        public DBContext()
        {
            
        }

        public static List<User> GetAllUsers()
        {
            conn = new OdbcConnection(connstring);
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
                            posts = reader.GetString(reader.GetOrdinal("posts")).Split(',').Select(i => int.Parse(i)).ToList(),
                            friends = reader.GetString(reader.GetOrdinal("friends")).Split(',').Select(i => Convert.ToString(i)).ToList()
                        });
                    }
                }
            }

            return users;
        }
    }
}
