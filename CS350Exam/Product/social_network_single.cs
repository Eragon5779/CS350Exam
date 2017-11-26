using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data;
using System.Security;
using System.Security.Cryptography;

namespace CS350Exam.Final
{
    public class social_network
    {

        public static List<User> users = new List<User>();
        public static List<Post> posts = new List<Post>();
        public static User currentUser;

        public static bool readUsers()
        {
            try
            {
                users = Server.GetAllUsers();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static bool addUser(string userID, string passSalt, string passHash, List<int> posts = null, List<string> friends = null)
        {
            try
            {
                if (!users.Contains(users.Find(user => user.userID == userID)))
                {

                    users.Add(new User()
                    {
                        userID = userID,
                        passSalt = passSalt,
                        passHash = passHash,
                        posts = posts ?? new List<int> { },
                        friends = friends ?? new List<string> { }
                    });

                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool writeUsers()
        {
            try
            {
                Server.WriteAllUsers(users);
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public static bool writePosts()
        {
            try
            {
                Server.WriteAllPosts(posts);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.Read();
                return false;
            }
        }

        public static bool writeAllData()
        {
            bool wroteUsers = writeUsers();
            bool wrotePosts = writePosts();
            return wroteUsers == wrotePosts == true;
        }

        public static bool loginUser(string userID, string password)
        {
            User tmp = users.Find(user => user.userID == userID);
            if (tmp.VerifyPassword(password))
            {
                currentUser = tmp;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool deleteUser(string userID)
        {
            try
            {
                
                User tmp = users.Find(user => user.userID == userID);
                if (tmp != null) {

                    Server.DeleteUser(tmp);
                    users.Remove(tmp);

                }

                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public static bool addFriend(string userID, string friend)
        {
            try
            {
                users.Find(user => user.userID == friend).AddFriend(userID);
                users.Find(user => user.userID == userID).AddFriend(friend);
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public static bool removeFriend(string userID, string friend)
        {
            try
            {
                users.Find(user => user.userID == friend).RemoveFriend(userID);
                users.Find(user => user.userID == userID).RemoveFriend(friend);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool resetData()
        {
            try
            {
                Server.ResetData();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool addPost(string userID, string content, string tagged)
        {
            try
            {
                int postID = posts.Count + 1;
                posts.Add(new Post()
                {
                    postID = postID,
                    opID = userID,
                    content = content,
                    timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
                    tagged = tagged
                });
                users.Find(user => user.userID == userID).AddPost(postID);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.Read();
                return false;
            }
            
        }

        public static bool readPosts()
        {
            try
            {
                posts = Server.GetAllPosts();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.Read();
                return false;
            }
        } 

        public static List<string> getFriends(string userID)
        {
            if (users.Contains(users.Find(user => user.userID == userID)))
            {
                return users.Find(user => user.userID == userID).friends;
            }
            else
            {
                return null;
            }
        }

        public static List<int> getFriendPosts(string userID)
        {
            if (users.Contains(users.Find(user => user.userID == userID)))
            {
                List<int> friendPosts = new List<int>();
                foreach (string friend in users.Find(user => user.userID == userID).friends)
                {
                    foreach (int postID in users.Find(user => user.userID == friend).posts)
                    {
                        friendPosts.Add(postID);
                    }
                }
                return friendPosts;
            }
            return null;
        }

        public static bool deletePost(string userID, int postID)
        {
            try
            {
                posts.Remove(posts.Find(post => post.postID == postID));
                users.Find(user => user.userID == userID).RemovePost(postID);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
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
    public class Post
    {

        public int postID { get; set; }
        public string opID { get; set; }
        public string content { get; set; }
        public string timeStamp { get; set; }
        public string tagged { get; set; }

    }
    public class User
    {
        
        public string userID { get; set; }
        public string passSalt { get; set; }
        public string passHash { get; set; }
        public List<int> posts { get; set; }
        public List<string> friends { get; set; }

        public bool VerifyPassword(string password)
        {
            return PassHash.VerifyPass(password, passSalt, passHash);
        }

        public void AddFriend(string friend)
        {
            if (!friends.Contains(friend))
            {
                friends.Add(friend);
            } 
            
        }

        public void RemoveFriend(string friend)
        {
            if (friends.Contains(friend))
            {
                friends.Remove(friend);
            }
        }

        public void AddPost(int id)
        {
            if (!posts.Contains(id))
            {
                posts.Add(id);
            }
            
        }

        public void RemovePost(int id)
        {
            if (posts.Contains(id))
            {
                posts.Remove(id);
            }

        }

    }
    class PassHash
    {

        private const int SaltByteLength = 64;
        private const int DerivedKeyLength = 128;

        public static string[] CreatePassHash(string password)
        {
            string[] saltHash = new string[2];

            byte[] salt = new byte[SaltByteLength];
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            csprng.GetBytes(salt);
            saltHash[0] = Convert.ToBase64String(salt);

            using (var hmac = new HMACSHA512())
            {
                var df = new Pbkdf2(hmac, Encoding.ASCII.GetBytes(password), salt, 30000);
                saltHash[1] = Convert.ToBase64String(df.GetBytes(64));
            }

            return saltHash;
        }

        public static bool VerifyPass(string password, string passSalt, string passHash)
        {
            string attemptHash;
            using (var hmac = new HMACSHA512())
            {
                var df = new Pbkdf2(hmac, Encoding.ASCII.GetBytes(password), Convert.FromBase64String(passSalt), 30000);
                attemptHash = Convert.ToBase64String(df.GetBytes(64));
            }
            return (attemptHash == passHash);

        }

    }
    public class Pbkdf2
    {

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public Pbkdf2(HMAC algorithm, Byte[] password, Byte[] salt, Int32 iterations)
        {
            if (algorithm == null) { throw new ArgumentNullException("algorithm", "Algorithm cannot be null."); }
            if (salt == null) { throw new ArgumentNullException("salt", "Salt cannot be null."); }
            if (password == null) { throw new ArgumentNullException("password", "Password cannot be null."); }
            this.Algorithm = algorithm;
            this.Algorithm.Key = password;
            this.Salt = salt;
            this.IterationCount = iterations;
            this.BlockSize = this.Algorithm.HashSize / 8;
            this.BufferBytes = new byte[this.BlockSize];
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public Pbkdf2(HMAC algorithm, Byte[] password, Byte[] salt)
            : this(algorithm, password, salt, 1000)
        {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public Pbkdf2(HMAC algorithm, String password, String salt, Int32 iterations) :
            this(algorithm, UTF8Encoding.UTF8.GetBytes(password), UTF8Encoding.UTF8.GetBytes(salt), iterations)
        {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public Pbkdf2(HMAC algorithm, String password, String salt) :
            this(algorithm, password, salt, 1000)
        {
        }


        private readonly int BlockSize;
        private uint BlockIndex = 1;

        private byte[] BufferBytes;
        private int BufferStartIndex = 0;
        private int BufferEndIndex = 0;


        /// <summary>
        /// Gets algorithm used for generating key.
        /// </summary>
        public HMAC Algorithm { get; private set; }

        /// <summary>
        /// Gets salt bytes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Byte array is proper return value in this case.")]
        public Byte[] Salt { get; private set; }

        /// <summary>
        /// Gets iteration count.
        /// </summary>
        public Int32 IterationCount { get; private set; }


        /// <summary>
        /// Returns a pseudo-random key from a password, salt and iteration count.
        /// </summary>
        /// <param name="count">Number of bytes to return.</param>
        /// <returns>Byte array.</returns>
        public Byte[] GetBytes(int count)
        {
            byte[] result = new byte[count];
            int resultOffset = 0;
            int bufferCount = this.BufferEndIndex - this.BufferStartIndex;

            if (bufferCount > 0)
            { //if there is some data in buffer
                if (count < bufferCount)
                { //if there is enough data in buffer
                    Buffer.BlockCopy(this.BufferBytes, this.BufferStartIndex, result, 0, count);
                    this.BufferStartIndex += count;
                    return result;
                }
                Buffer.BlockCopy(this.BufferBytes, this.BufferStartIndex, result, 0, bufferCount);
                this.BufferStartIndex = this.BufferEndIndex = 0;
                resultOffset += bufferCount;
            }

            while (resultOffset < count)
            {
                int needCount = count - resultOffset;
                this.BufferBytes = this.Func();
                if (needCount > this.BlockSize)
                { //we one (or more) additional passes
                    Buffer.BlockCopy(this.BufferBytes, 0, result, resultOffset, this.BlockSize);
                    resultOffset += this.BlockSize;
                }
                else
                {
                    Buffer.BlockCopy(this.BufferBytes, 0, result, resultOffset, needCount);
                    this.BufferStartIndex = needCount;
                    this.BufferEndIndex = this.BlockSize;
                    return result;
                }
            }
            return result;
        }


        private byte[] Func()
        {
            var hash1Input = new byte[this.Salt.Length + 4];
            Buffer.BlockCopy(this.Salt, 0, hash1Input, 0, this.Salt.Length);
            Buffer.BlockCopy(GetBytesFromInt(this.BlockIndex), 0, hash1Input, this.Salt.Length, 4);
            var hash1 = this.Algorithm.ComputeHash(hash1Input);

            byte[] finalHash = hash1;
            for (int i = 2; i <= this.IterationCount; i++)
            {
                hash1 = this.Algorithm.ComputeHash(hash1, 0, hash1.Length);
                for (int j = 0; j < this.BlockSize; j++)
                {
                    finalHash[j] = (byte)(finalHash[j] ^ hash1[j]);
                }
            }
            if (this.BlockIndex == uint.MaxValue) { throw new InvalidOperationException("Derived key too long."); }
            this.BlockIndex += 1;

            return finalHash;
        }

        private static byte[] GetBytesFromInt(uint i)
        {
            var bytes = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            }
            else
            {
                return bytes;
            }
        }

    }
}
