using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS350Exam.Product;
using System.Diagnostics;

namespace CS350Exam
{
    class Program
    {

        public static List<User> users;
        public static List<Post> posts;
        public static string currentUser;

        static void Main(string[] args)
        {

            //users = DBContext.GetAllUsers();

            //Console.WriteLine(
            //   "\nUser ID: " + users[0].userID + 
            //    "\nPass Salt: " + users[0].passSalt +
            //    "\nPass Hash: " + users[0].passHash +
            //    "\nPosts: " + users[0].posts +
            //    "\nFriends: " + users[0].friends
            //    );
            //User temp = users[0];
            //Console.WriteLine(PassHash.VerifyPass("@rgetlam5779", temp.passSalt, temp.passHash));
            //Console.ReadLine();

            test_readUsers();

        }

        public static bool readUsers()
        {
            try
            {
                users = DBContext.GetAllUsers();
                return true;
            } catch
            {
                return false;
            }
            
        }
        public static void test_readUsers()
        {
            Debug.Assert(readUsers() == true);
        }


    }
}
