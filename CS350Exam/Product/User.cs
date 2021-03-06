﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Odbc;

namespace CS350Exam.Product
{
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
}
