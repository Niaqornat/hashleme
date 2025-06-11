using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using hashleme.Models;

namespace hashleme.Controllers
{
    public class AESController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Encrypt(string text, string? key = null)
        {
            try
            {
                AESCipher aes;
                if (!string.IsNullOrEmpty(key))
                {
                    byte[] keyBytes = Convert.FromBase64String(key);
                    aes = new AESCipher(keyBytes);
                }
                else
                {
                    aes = new AESCipher();
                }

                (byte[] encryptedData, byte[] iv) = aes.Encrypt(text);

                return Json(new
                {
                    success = true,
                    encryptedText = Convert.ToBase64String(encryptedData),
                    iv = Convert.ToBase64String(iv),
                    key = Convert.ToBase64String(aes.Key)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Decrypt(string encryptedText, string iv, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedText) || string.IsNullOrEmpty(iv) || string.IsNullOrEmpty(key))
                {
                    return Json(new { success = false, message = "Tüm alanları doldurunuz." });
                }

                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] ivBytes = Convert.FromBase64String(iv);
                byte[] keyBytes = Convert.FromBase64String(key);

                var aes = new AESCipher();
                string decryptedText = aes.Decrypt(encryptedBytes, ivBytes, keyBytes);

                return Json(new { success = true, decryptedText });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
