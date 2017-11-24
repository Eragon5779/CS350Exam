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
        public static OdbcConnection conn = new OdbcConnection();
        public static string connString = "Driver={MySQL ODBC 5.3 UNICODE Driver};" +
                                          "Server=dragonfirecomputing.com;" +
                                          "Database=eragon57_cs350;" +
                                          "User=eragon57_readdb;" +
                                          "Password=Ce2GoMCdneDEQGAv5dKVQl95XiTHD0QM;" +
                                          "OPTION=3";

        public static List<User> GetAllUsers()
        {
            conn.ConnectionString = connString;
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

        public static List<Post> GetAllPosts()
        {
            conn.ConnectionString = connString;
            conn.Open();
            List<Post> posts = new List<Post>();

            using (conn)
            {
                OdbcCommand cmd = new OdbcCommand("SELECT * from post", conn);
                using (OdbcDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        posts.Add(new Post()
                        {
                            postID = reader.GetInt32(reader.GetOrdinal("ID")),
                            opID = reader.GetString(reader.GetOrdinal("opID")),
                            content = reader.GetString(reader.GetOrdinal("content")),
                            timeStamp = reader.GetString(reader.GetOrdinal("timeStamp")),
                            tagged = reader.GetString(reader.GetOrdinal("tagged"))
                        });
                    }
                }
            }
            conn.Close();
            return posts;
        }

        public static void WriteAllUsers(List<User> users)
        {
            conn.ConnectionString = connString;
            conn.Open();
            using (conn)
            {
                if (users != null)
                {
                    foreach (User user in users)
                    {
                        string friends = "";
                        switch (user.friends.Count)
                        {
                            case 0: friends = ""; break;
                            case 1: friends = user.friends[0]; break;
                            default: friends = string.Join(",", user.friends); break;
                        }
                        string posts = "";
                        switch (user.posts.Count)
                        {
                            case 0: posts = ""; break;
                            case 1: posts = Convert.ToString(user.posts[0]); break;
                            default: posts = string.Join(",", user.posts); break;
                        }

                        using (OdbcCommand cmd = new OdbcCommand("INSERT INTO user (userID, passSalt, passHash, posts, friends) VALUES (\"" +
                                                                  user.userID + "\",\"" + user.passSalt + "\",\"" + user.passHash + "\",\"" + posts + "\",\"" + friends + "\") " + 
                                                                  "ON DUPLICATE KEY UPDATE posts = \"" + posts + "\", friends = \"" + friends + "\""))
                        {
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

            }
            conn.Close();
        }

        public static void WriteAllPosts(List<Post> posts)
        {
            conn.ConnectionString = connString;
            conn.Open();
            using (conn)
            {
                if (posts != null)
                {
                   foreach (Post post in posts)
                    {

                        using (OdbcCommand cmd = new OdbcCommand("INSERT IGNORE INTO post (ID, opID, content, timeStamp, tagged) VALUES (\"" +
                                                                  post.postID + "\",\"" + post.opID + "\",\"" + post.content + "\",\"" + post.timeStamp + "\", \"" + post.tagged + "\")" ))
                        {
                            cmd.Connection = conn;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
 
            }
            conn.Close();
        }

        public static void DeleteUser(User user)
        {
            conn.ConnectionString = connString;
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

        public static void DeletePost(Post post)
        {
            conn.ConnectionString = connString;
            conn.Open();

            using (conn)
            {
                using (OdbcCommand cmd = new OdbcCommand("DELETE FROM post WHERE ID = \"" + post.postID + "\""))
                {
                    cmd.Connection = conn;
                    cmd.ExecuteNonQuery();
                }
            }

            conn.Close();
        }

        public static void ResetData()
        {
            conn.ConnectionString = connString;
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
