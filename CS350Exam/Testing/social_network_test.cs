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
            test_addUser();
            test_loginUser();
            test_addFriend();
            test_writeUsers();

        }

        public static void test_readUsers()
        {
            Debug.Assert(Product.social_network.readUsers());
        }

        public static void test_addUser()
        {
            string[] saltHash = PassHash.CreatePassHash("test");
            Debug.Assert(social_network.addUser("test", saltHash[0], saltHash[1], new List<int>() { }, new List<string>() { }));
        }

        public static void test_writeUsers()
        {
            Debug.Assert(social_network.writeUsers());
        }

        public static void test_loginUser()
        {
            Debug.Assert(social_network.loginUser("test", "test"));
        }

        public static void test_addFriend()
        {
            string[] saltHash = PassHash.CreatePassHash("test");
            social_network.addUser("test2", saltHash[0], saltHash[1], new List<int>() { }, new List<string>() { });

            Debug.Assert(social_network.addFriend("test", "test2"));
        }

        public static void test_resetData()
        {
            Debug.Assert(social_network.resetData());
        }


    }
}
