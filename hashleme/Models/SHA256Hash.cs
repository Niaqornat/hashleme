using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace hashleme.Models
{
    public class SHA256Hash
    {
        public static string HashText(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Özeti alınacak metin boş olamaz.");

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public static string HashFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Özeti alınacak dosya bulunamadı.", filePath);

            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public static bool VerifyTextHash(string text, string hashValue)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Doğrulanacak metin boş olamaz.");

            if (string.IsNullOrEmpty(hashValue))
                throw new ArgumentException("Özet değeri boş olamaz.");

            string calculatedHash = HashText(text);
            return string.Equals(calculatedHash, hashValue, StringComparison.OrdinalIgnoreCase);
        }

        public static bool VerifyFileHash(string filePath, string hashValue)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Doğrulanacak dosya bulunamadı.", filePath);

            if (string.IsNullOrEmpty(hashValue))
                throw new ArgumentException("Özet değeri boş olamaz.");

            string calculatedHash = HashFile(filePath);
            return string.Equals(calculatedHash, hashValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
