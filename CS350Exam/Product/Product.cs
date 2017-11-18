using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS350Exam.Product
{
    public class Product
    {

        public static List<User> users;
        public static List<Post> posts;
        public static string currentUser;

        public static bool readUsers()
        {
            try
            {
                users = DBContext.GetAllUsers();
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
                DBContext.WriteAllUsers(users);
                return true;
            } catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
                Console.Read();
                return false;
            }
            
        }

        public static bool loginUser(string username, string password)
        {
            return users.Find(user => user.userID == username).VerifyPassword(password);
        }

    }
}
