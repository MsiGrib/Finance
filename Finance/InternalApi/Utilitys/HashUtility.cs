using System.Security.Cryptography;
using System.Text;

namespace InternalApi.Utilitys
{
    public static class HashUtility
    {
        private const int _saltSize = 16;

        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Пустные данные!");
            }

            byte[] salt = GenerateSalt();
            byte[] hash = ComputeHash(password, salt);

            return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string hashPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashPassword))
            {
                throw new Exception("Пустные данные!");
            }

            string[] parts = hashPassword.Split('.');
            if (parts.Length != 2)
                return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hash = Convert.FromBase64String(parts[1]);

            byte[] computedHash = ComputeHash(password, salt);

            return SlowEquals(hash, computedHash);
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[_saltSize];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        private static byte[] ComputeHash(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combined = new byte[salt.Length + passwordBytes.Length];

                Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
                Buffer.BlockCopy(passwordBytes, 0, combined, salt.Length, passwordBytes.Length);

                return sha256.ComputeHash(combined);
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            int diff = a.Length ^ b.Length;

            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }
    }
}
