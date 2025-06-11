using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using hashleme.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hashleme.Controllers
{
    // SHA256 özet işlemleri için controller
    // Metin ve dosya özeti alma ve doğrulama işlemlerini yönetir
    public class SHA256Controller : Controller
    {
        // Ana sayfa görünümü
        public ActionResult Index()
        {
            return View();
        }

        // Metin için SHA256 özeti oluşturur
        // Gelen metni alıp hash'ini döndürür
        [HttpPost]
        public ActionResult HashText(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                {
                    return Json(new
                    {
                        success = false,
                        error = "Metin boş olamaz."
                    });
                }

                string hash = SHA256Hash.HashText(text);

                return Json(new
                {
                    success = true,
                    hash = hash
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // Metin ve hash değerini karşılaştırır
        // Doğrulama sonucunu true/false olarak döndürür
        [HttpPost]
        public ActionResult VerifyText(string text, string hash)
        {
            try
            {
                if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(hash))
                {
                    return Json(new
                    {
                        success = false,
                        error = "Metin ve özet değeri gereklidir."
                    });
                }

                bool isValid = SHA256Hash.VerifyTextHash(text, hash);

                return Json(new
                {
                    success = true,
                    is_valid = isValid
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // Dosya için SHA256 özeti oluşturur
        // Dosyayı geçici olarak kaydedip işlem sonrası siliyoruz
        [HttpPost]
        public async Task<ActionResult> HashFile()
        {
            try
            {
                if (Request.Form.Files.Count == 0 || Request.Form.Files[0] == null)
                {
                    return Json(new
                    {
                        success = false,
                        error = "Dosya yüklenemedi."
                    });
                }

                var file = Request.Form.Files[0];
                if (string.IsNullOrEmpty(file.FileName))
                {
                    return Json(new
                    {
                        success = false,
                        error = "Dosya seçilmedi."
                    });
                }

                string tempDir = Path.GetTempPath();
                string filePath = Path.Combine(tempDir, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string hash = SHA256Hash.HashFile(filePath);

                System.IO.File.Delete(filePath);

                return Json(new
                {
                    success = true,
                    filename = file.FileName,
                    hash = hash
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // Dosya hash'ini verilen hash ile karşılaştırır
        // Dosyayı geçici olarak kaydedip işlem sonrası siliyoruz
        [HttpPost]
        public async Task<ActionResult> VerifyFile(string hash)
        {
            try
            {
                if (Request.Form.Files.Count == 0 || Request.Form.Files[0] == null)
                {
                    return Json(new
                    {
                        success = false,
                        error = "Dosya yüklenemedi."
                    });
                }

                var file = Request.Form.Files[0];
                if (string.IsNullOrEmpty(file.FileName))
                {
                    return Json(new
                    {
                        success = false,
                        error = "Dosya seçilmedi."
                    });
                }

                if (string.IsNullOrEmpty(hash))
                {
                    return Json(new
                    {
                        success = false,
                        error = "Özet değeri gereklidir."
                    });
                }

                string tempDir = Path.GetTempPath();
                string filePath = Path.Combine(tempDir, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                bool isValid = SHA256Hash.VerifyFileHash(filePath, hash);

                System.IO.File.Delete(filePath);

                return Json(new
                {
                    success = true,
                    filename = file.FileName,
                    is_valid = isValid
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }
    }
}
