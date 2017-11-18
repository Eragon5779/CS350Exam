﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CS350Exam.Product
{
    public class Pbkdf2
    {

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public Pbkdf2(HMAC algorithm, Byte[] password, Byte[] salt, Int32 iterations)
        {
            if (algorithm == null) { throw new ArgumentNullException("algorithm", "Algorithm cannot be null."); }
            if (salt == null) { throw new ArgumentNullException("salt", "Salt cannot be null."); }
            if (password == null) { throw new ArgumentNullException("password", "Password cannot be null."); }
            this.Algorithm = algorithm;
            this.Algorithm.Key = password;
            this.Salt = salt;
            this.IterationCount = iterations;
            this.BlockSize = this.Algorithm.HashSize / 8;
            this.BufferBytes = new byte[this.BlockSize];
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public Pbkdf2(HMAC algorithm, Byte[] password, Byte[] salt)
            : this(algorithm, password, salt, 1000)
        {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <param name="iterations">The number of iterations for the operation.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public Pbkdf2(HMAC algorithm, String password, String salt, Int32 iterations) :
            this(algorithm, UTF8Encoding.UTF8.GetBytes(password), UTF8Encoding.UTF8.GetBytes(salt), iterations)
        {
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="algorithm">HMAC algorithm to use.</param>
        /// <param name="password">The password used to derive the key.</param>
        /// <param name="salt">The key salt used to derive the key.</param>
        /// <exception cref="System.ArgumentNullException">Algorithm cannot be null - Password cannot be null. -or- Salt cannot be null.</exception>
        public Pbkdf2(HMAC algorithm, String password, String salt) :
            this(algorithm, password, salt, 1000)
        {
        }


        private readonly int BlockSize;
        private uint BlockIndex = 1;

        private byte[] BufferBytes;
        private int BufferStartIndex = 0;
        private int BufferEndIndex = 0;


        /// <summary>
        /// Gets algorithm used for generating key.
        /// </summary>
        public HMAC Algorithm { get; private set; }

        /// <summary>
        /// Gets salt bytes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Byte array is proper return value in this case.")]
        public Byte[] Salt { get; private set; }

        /// <summary>
        /// Gets iteration count.
        /// </summary>
        public Int32 IterationCount { get; private set; }


        /// <summary>
        /// Returns a pseudo-random key from a password, salt and iteration count.
        /// </summary>
        /// <param name="count">Number of bytes to return.</param>
        /// <returns>Byte array.</returns>
        public Byte[] GetBytes(int count)
        {
            byte[] result = new byte[count];
            int resultOffset = 0;
            int bufferCount = this.BufferEndIndex - this.BufferStartIndex;

            if (bufferCount > 0)
            { //if there is some data in buffer
                if (count < bufferCount)
                { //if there is enough data in buffer
                    Buffer.BlockCopy(this.BufferBytes, this.BufferStartIndex, result, 0, count);
                    this.BufferStartIndex += count;
                    return result;
                }
                Buffer.BlockCopy(this.BufferBytes, this.BufferStartIndex, result, 0, bufferCount);
                this.BufferStartIndex = this.BufferEndIndex = 0;
                resultOffset += bufferCount;
            }

            while (resultOffset < count)
            {
                int needCount = count - resultOffset;
                this.BufferBytes = this.Func();
                if (needCount > this.BlockSize)
                { //we one (or more) additional passes
                    Buffer.BlockCopy(this.BufferBytes, 0, result, resultOffset, this.BlockSize);
                    resultOffset += this.BlockSize;
                }
                else
                {
                    Buffer.BlockCopy(this.BufferBytes, 0, result, resultOffset, needCount);
                    this.BufferStartIndex = needCount;
                    this.BufferEndIndex = this.BlockSize;
                    return result;
                }
            }
            return result;
        }


        private byte[] Func()
        {
            var hash1Input = new byte[this.Salt.Length + 4];
            Buffer.BlockCopy(this.Salt, 0, hash1Input, 0, this.Salt.Length);
            Buffer.BlockCopy(GetBytesFromInt(this.BlockIndex), 0, hash1Input, this.Salt.Length, 4);
            var hash1 = this.Algorithm.ComputeHash(hash1Input);

            byte[] finalHash = hash1;
            for (int i = 2; i <= this.IterationCount; i++)
            {
                hash1 = this.Algorithm.ComputeHash(hash1, 0, hash1.Length);
                for (int j = 0; j < this.BlockSize; j++)
                {
                    finalHash[j] = (byte)(finalHash[j] ^ hash1[j]);
                }
            }
            if (this.BlockIndex == uint.MaxValue) { throw new InvalidOperationException("Derived key too long."); }
            this.BlockIndex += 1;

            return finalHash;
        }

        private static byte[] GetBytesFromInt(uint i)
        {
            var bytes = BitConverter.GetBytes(i);
            if (BitConverter.IsLittleEndian)
            {
                return new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
            }
            else
            {
                return bytes;
            }
        }

    }
}
