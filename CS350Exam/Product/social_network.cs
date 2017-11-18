using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS350Exam.Product
{
    public class social_network
    {

        public static List<User> users;
        public static List<Post> posts;
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
                users.Add(new User()
                {
                    userID = userID,
                    passSalt = passSalt,
                    passHash = passHash,
                    posts = posts == null ? new List<int> { } : posts,
                    friends = friends == null ? new List<string> { } : friends
                });
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
            } catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
                Console.Read();
                return false;
            }
            
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
            } catch
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
            } catch
            {
                return false;
            }
            
        }

    }
}
