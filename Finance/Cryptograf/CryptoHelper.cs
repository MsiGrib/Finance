using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptograf
{
    public static class CryptoHelper
    {
        public static string Encrypt(string plainText, string key)
        {
            if (plainText == null)
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key must be non-empty", nameof(key));

            var sb = new StringBuilder();
            for (int i = 0; i < plainText.Length; i++)
            {
                sb.Append(plainText[i]);
                sb.Append(key[i % key.Length]);
            }
            sb.Append(key);

            return new string(sb.ToString().Reverse().ToArray());
        }

        public static string Decrypt(string cipherText, string key)
        {
            if (cipherText == null)
                throw new ArgumentNullException(nameof(cipherText));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key must be non-empty", nameof(key));

            var reversed = new string(cipherText.Reverse().ToArray());
            if (!reversed.EndsWith(key))
                throw new InvalidOperationException("Invalid key or corrupted data");
            var trimmed = reversed.Substring(0, reversed.Length - key.Length);

            var plainSb = new StringBuilder();
            for (int i = 0; i < trimmed.Length; i += 2)
                plainSb.Append(trimmed[i]);

            return plainSb.ToString();
        }
    }
}
