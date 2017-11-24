using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS350Exam.Product
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
}
