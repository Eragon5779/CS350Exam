using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;

namespace CS350Exam.Product
{
    class PassHash
    {

        private const int SaltByteLength = 64;
        private const int DerivedKeyLength = 128;

        public static string[] CreatePassHash(string password)
        {
            string[] saltHash = new string[2];

            byte[] salt = new byte[SaltByteLength];
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            csprng.GetBytes(salt);
            saltHash[0] = Convert.ToBase64String(salt);

            using (var hmac = new HMACSHA512())
            {
                var df = new Pbkdf2(hmac, Encoding.ASCII.GetBytes(password), salt, 30000);
                saltHash[1] = Convert.ToBase64String(df.GetBytes(64));
            }

            return saltHash;
        }

        public static bool VerifyPass(string password, string passSalt, string passHash)
        {
            string attemptHash;
            using (var hmac = new HMACSHA512())
            {
                var df = new Pbkdf2(hmac, Encoding.ASCII.GetBytes(password), Convert.FromBase64String(passSalt), 30000);
                attemptHash = Convert.ToBase64String(df.GetBytes(64));
            }
            Console.WriteLine("\n\n" + attemptHash + "\n" + passHash + "\n\n");
            return (attemptHash == passHash);

        }

    }
}
