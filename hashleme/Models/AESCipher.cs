using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace hashleme.Models
{
    public class AESCipher
    {
        private const int KeySize = 32; // AES-256 için anahtar boyutu (32 byte = 256 bit)
        private const int BlockSize = 16; // AES için blok boyutu (16 byte = 128 bit)
        private readonly byte[] _key;

        public byte[] Key => _key;

        public AESCipher(byte[]? key = null)
        {
            _key = key ?? GenerateRandomKey();
        }

        private byte[] GenerateRandomKey()
        {
            var key = new byte[KeySize];
            RandomNumberGenerator.Fill(key);
            return key;
        }

        public (byte[] encryptedData, byte[] iv) Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Şifrelenecek metin boş olamaz.");

            byte[] iv;
            byte[] encryptedData;

            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize * 8; // bit cinsinden
                aes.BlockSize = BlockSize * 8; // bit cinsinden
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = _key;
                aes.GenerateIV();
                iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    encryptedData = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                }
            }

            return (encryptedData, iv);
        }

        public string Decrypt(byte[] encryptedData, byte[] iv, byte[] key)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize * 8;
                aes.BlockSize = BlockSize * 8;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}
