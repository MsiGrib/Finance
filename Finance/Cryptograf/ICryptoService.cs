using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptograf
{
    public interface ICryptoService
    {
        public byte[] Encrypt(byte[] data);
        public byte[] Decrypt(byte[] cipher);
        public string EncryptString(string plainText);
        public string DecryptString(string cipherText);
    }
}
