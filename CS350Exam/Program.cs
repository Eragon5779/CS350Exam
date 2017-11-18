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
            test_writeUsers();
            test_loginUser();
            //Product.Product.writeUsers();

        }

        public static void test_readUsers()
        {
            Debug.Assert(Product.Product.readUsers());
        }

        public static void test_addUser()
        {
            string[] saltHash = PassHash.CreatePassHash("test");
            Debug.Assert(Product.Product.addUser("test", saltHash[0], saltHash[1], new List<int>() { }, new List<string>() { }));
        }

        public static void test_writeUsers()
        {
            Debug.Assert(Product.Product.writeUsers());
        }

        public static void test_loginUser()
        {
            Debug.Assert(Product.Product.loginUser("test", "test"));
        }


    }
}
