using System;
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

    }
}
