using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS350Exam.Product;
using System.Diagnostics;

namespace CS350Exam.Testing
{
    class social_network_test
    {

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
            test_addUser("test", "test");
            test_loginUser("test", "test");
            test_addUser("test2", "test");
            test_addFriend("test", "test2");
            test_addUser("test3", "test");
            test_addFriend("test", "test3");
            test_removeFriend("test", "test2");
            test_writeUsers();

        }

        public static void test_readUsers()
        {
            Debug.Assert(Product.social_network.readUsers());
        }

        public static void test_addUser(string userID, string password)
        {
            string[] saltHash = PassHash.CreatePassHash(password);
            Debug.Assert(social_network.addUser(userID, saltHash[0], saltHash[1], new List<int>() { }, new List<string>() { }));
        }

        public static void test_writeUsers()
        {
            Debug.Assert(social_network.writeUsers());
        }

        public static void test_loginUser(string userID, string password)
        {
            Debug.Assert(social_network.loginUser(userID, password));
        }

        public static void test_addFriend(string userID, string friendID)
        {
            Debug.Assert(social_network.addFriend(userID, friendID));
        }

        public static void test_removeFriend(string userID, string friendID)
        {
            Debug.Assert(social_network.removeFriend(userID, friendID));
        }

        public static void test_resetData()
        {
            Debug.Assert(social_network.resetData());
        }


    }
}
