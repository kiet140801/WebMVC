using System.Security.Cryptography;
using System.Text;

namespace App.Core.Commons
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // Size of salt in bytes
        private const int HashSize = 20; // Size of hash in bytes
        private const int Iterations = 10000; // Number of iterations

        public (string Hash, string Salt) HashPassword(string password)
        {
            // Generate a random salt
            var salt = GenerateSalt();

            // Create the hash
            var hash = ComputeHash(password, salt);

            // Return the hash and salt as Base64 encoded strings
            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public bool VerifyPassword(string password, string salt, string hash)
        {
            // Convert Base64 strings to byte arrays
            var saltBytes = Convert.FromBase64String(salt);
            var hashBytes = Convert.FromBase64String(hash);

            // Compute the hash of the input password with the same salt
            var computedHash = ComputeHash(password, saltBytes);

            // Compare the computed hash with the stored hash
            return AreHashesEqual(computedHash, hashBytes);
        }

        private byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[SaltSize];
                rng.GetBytes(salt);
                return salt;
            }
        }

        private byte[] ComputeHash(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        private bool AreHashesEqual(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }

            return true;
        }
    }
}
