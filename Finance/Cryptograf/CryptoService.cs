using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cryptograf
{
    public class CryptoService : ICryptoService
    {
        private const string _key = "Gv1n3fDw5hJz2Y6aT9Vu7w==";

        public CryptoService() { }

        public byte[] Encrypt(byte[] data)
        {
            string plainText = Encoding.UTF8.GetString(data);
            string cipherText = CryptoHelper.Encrypt(plainText, _key);

            return Encoding.UTF8.GetBytes(cipherText);
        }

        public byte[] Decrypt(byte[] cipher)
        {
            string cipherText = Encoding.UTF8.GetString(cipher);
            string plainText = CryptoHelper.Decrypt(cipherText, _key);

            return Encoding.UTF8.GetBytes(plainText);
        }

        public string EncryptString(string plainText)
            => CryptoHelper.Encrypt(plainText, _key);

        public string DecryptString(string cipherText)
            => CryptoHelper.Decrypt(cipherText, _key);
    }
}
