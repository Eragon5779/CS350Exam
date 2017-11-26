using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS350Exam.Final;
using System.Diagnostics;

namespace CS350Exam.Testing
{
    class social_network_test
    {

        static void Main(string[] args)
        {
            test_readAllData();
            test_addUser("test", "test");
            test_loginUser("test", "test");
            test_addUser("test2", "test");
            test_addFriend("test", "test2");
            test_addUser("test3", "test");
            test_addFriend("test", "test3");
            test_removeFriend("test", "test2");
            test_getFriends("test");
            test_addPost("test", "This is a test post");
            // Test add post with tagged friend
            test_addPost("test", "This is a test tag", "test3");
            test_getFriendPosts("test");
            test_deletePost("test", 1);
            test_writeAllData();
            test_resetData();
        }

        public static void test_readUsers()
        {
            Debug.Assert(Final.social_network.readUsers());
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

        public static void test_getFriends(string userID)
        {
            Debug.Assert(social_network.getFriends(userID) != null);
        }

        public static void test_addPost(string userID, string content, string tagged = "")
        {
            Debug.Assert(social_network.addPost(userID, content, tagged));
        }

        public static void test_readPosts()
        {
            Debug.Assert(social_network.readPosts());
        }

        public static void test_writePosts()
        {
            Debug.Assert(social_network.writePosts())
;        }

        public static void test_readAllData()
        {
            test_readUsers();
            test_readPosts();
        }

        public static void test_writeAllData()
        {
            Debug.Assert(social_network.writeAllData());
        }

        public static void test_getFriendPosts(string userID)
        {
            Debug.Assert(social_network.getFriendPosts(userID) != null);
        }

        public static void test_resetData()
        {
            Debug.Assert(social_network.resetData());
        }

        public static void test_deletePost(string userID, int postID)
        {
            Debug.Assert(social_network.deletePost(userID, postID));
        }

        public static void test_deleteUser(string userID)
        {
            Debug.Assert(social_network.deleteUser(userID));
        }
    }
}
