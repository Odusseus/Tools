using System.Security.Cryptography;
using System.Text;

namespace metalimes.Services
{
    /// <summary>
    /// Service for encrypting and decrypting sensitive data using AES encryption.
    /// </summary>
    public class EncryptionService
    {
        /// <summary>
        /// Encrypts a plaintext string using AES-256-CBC with a provided key.
        /// </summary>
        /// <param name="plaintext">The text to encrypt</param>
        /// <param name="key">The encryption key (should be 32 bytes for AES-256)</param>
        /// <returns>Base64 encoded ciphertext with IV prepended</returns>
        public static string Encrypt(string plaintext, string key)
        {
            if (string.IsNullOrEmpty(plaintext))
                throw new ArgumentException("Plaintext cannot be null or empty.", nameof(plaintext));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            // Derive a 32-byte key from the provided string using SHA-256
            byte[] keyBytes = DeriveKey(key, 32);

            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = keyBytes;

                // Generate a random IV
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv))
                {
                    byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                    byte[] cipherBytes = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);

                    // Combine IV and ciphertext, then Base64 encode
                    byte[] result = new byte[iv.Length + cipherBytes.Length];
                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }

        /// <summary>
        /// Decrypts a Base64 encoded AES-encrypted string.
        /// </summary>
        /// <param name="ciphertext">The Base64 encoded ciphertext with IV prepended</param>
        /// <param name="key">The encryption key (should be 32 bytes for AES-256)</param>
        /// <returns>The decrypted plaintext</returns>
        public static string Decrypt(string ciphertext, string key)
        {
            if (string.IsNullOrEmpty(ciphertext))
                throw new ArgumentException("Ciphertext cannot be null or empty.", nameof(ciphertext));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            byte[] keyBytes = DeriveKey(key, 32);

            try
            {
                byte[] buffer = Convert.FromBase64String(ciphertext);

                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Key = keyBytes;

                    // Extract IV (first 16 bytes for AES)
                    byte[] iv = new byte[16];
                    Buffer.BlockCopy(buffer, 0, iv, 0, 16);
                    aes.IV = iv;

                    // Extract ciphertext
                    byte[] cipherBytes = new byte[buffer.Length - 16];
                    Buffer.BlockCopy(buffer, 16, cipherBytes, 0, cipherBytes.Length);

                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv))
                    {
                        byte[] plaintextBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(plaintextBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to decrypt ciphertext.", ex);
            }
        }

        /// <summary>
        /// Derives a fixed-length key from a string using SHA-256.
        /// </summary>
        /// <param name="input">The input string</param>
        /// <param name="keyLength">The desired key length in bytes</param>
        /// <returns>A byte array of the specified length</returns>
        private static byte[] DeriveKey(string input, int keyLength)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // If more bytes are needed than SHA-256 provides (32 bytes), use HKDF-like approach
                if (keyLength <= hash.Length)
                {
                    byte[] result = new byte[keyLength];
                    Buffer.BlockCopy(hash, 0, result, 0, keyLength);
                    return result;
                }

                // For longer keys, concatenate multiple hash iterations
                List<byte> keyBytes = new List<byte>(hash);
                int iteration = 1;
                while (keyBytes.Count < keyLength)
                {
                    byte[] iterationBytes = Encoding.UTF8.GetBytes(input + iteration);
                    byte[] nextHash = sha256.ComputeHash(iterationBytes);
                    keyBytes.AddRange(nextHash);
                    iteration++;
                }

                byte[] finalKey = new byte[keyLength];
                Buffer.BlockCopy(keyBytes.ToArray(), 0, finalKey, 0, keyLength);
                return finalKey;
            }
        }
    }
}
