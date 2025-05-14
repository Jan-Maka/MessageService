using Microsoft.AspNet.Identity;
using System.Security.Cryptography;

namespace Project.Server.Components
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            // Split the stored hashed password into hash and salt
            var parts = hashedPassword.Split('-');
            if (parts.Length != 2)
            {
                return PasswordVerificationResult.Failed;
            }

            // Convert the hash and salt from hex to byte arrays
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);

            // Hash the provided password with the same salt
            byte[] providedHash = Rfc2898DeriveBytes.Pbkdf2(providedPassword, salt, Iterations, Algorithm, HashSize);

            // Compare the computed hash with the stored hash
            for (int i = 0; i < hash.Length; i++)
            {
                if (hash[i] != providedHash[i])
                {
                    return PasswordVerificationResult.Failed;
                }
            }

            return PasswordVerificationResult.Success;
        }
    }
}
