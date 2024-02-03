using JsonFormatterPlus;
using Pretech.Software.RConfig.Crypto.Abstract;
using System.Security.Cryptography;
using System.Text;

namespace Pretech.Software.RConfig.Crypto.Concrete
{
    public class JsonCrypto : IJsonCrypto
    {


        public async Task<ExecuteResponse<string>> DecryptAsync(string value, string key)
        {
            ExecuteResponse<string> response = new();
            try
            {
                // Base64 formatındaki şifrelenmiş veriyi byte dizisine çevirin
                byte[] cipherTextBytes = Convert.FromBase64String(value);

                // IV'yi ayırın
                byte[] iv = new byte[16];
                Buffer.BlockCopy(cipherTextBytes, 0, iv, 0, iv.Length);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(key);
                    aesAlg.IV = iv;

                    // Çözme için bir AES şifreleme nesnesi oluşturun
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream())
                    {
                        // Çözme işlemini gerçekleştirin
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                        {
                            await csDecrypt.WriteAsync(cipherTextBytes, iv.Length, cipherTextBytes.Length - iv.Length);
                        }

                        // Çözülmüş veriyi string formatına çevirin
                        var json = (Encoding.UTF8.GetString(msDecrypt.ToArray()));
                        response.Success(JsonFormatter.Format(json));
                    }
                }

            }
            catch (Exception ex)
            {

                response.Error(ex);
            }
            return response;
        }

        public async Task<ExecuteResponse<string>> EncryptAsync(string value, string key)
        {
            ExecuteResponse<string> response = new();
            try
            {
                value = JsonFormatter.Format(value);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(key);

                    // Şifreleme için bir AES şifreleme nesnesi oluşturun
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        // Şifreleme işlemini gerçekleştirin
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                await swEncrypt.WriteAsync(value);
                            }
                        }

                        // IV'yi şifrelenmiş metne ekleyin
                        byte[] encryptedText = msEncrypt.ToArray();
                        byte[] result = new byte[aesAlg.IV.Length + encryptedText.Length];
                        Buffer.BlockCopy(aesAlg.IV, 0, result, 0, aesAlg.IV.Length);
                        Buffer.BlockCopy(encryptedText, 0, result, aesAlg.IV.Length, encryptedText.Length);

                        // Şifrelenmiş veriyi Base64 formatına çevirin
                        response.Success(Convert.ToBase64String(result));
                    }
                }
            }
            catch (Exception ex)
            {
                response.Error(ex);
            }
            return response;
        }

        public async Task<string> FromBase64(string base64)
        {
            var task = Task.Run(() =>
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            });
            return await task;
        }

        public async Task<ExecuteResponse<string>> GenerateKeyAsync()
        {
            ExecuteResponse<string> response = new();
            try
            {
                var task = Task.Run(() =>
                {
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        byte[] keyData = new byte[32]; // 256 bit uzunluğunda bir anahtar kullanılıyor
                        rng.GetBytes(keyData);
                        var key = Convert.ToBase64String(keyData);
                        response.Success(key.Substring(0, 32));
                    }


                });
                await task;
            }
            catch (Exception ex)
            {
                response.Error(ex);
            }
            return response;
        }

        public async Task<string> ToBase64(string value)
        {
            var task = Task.Run(() =>
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(value);
                return System.Convert.ToBase64String(plainTextBytes);


            });
            return await task;
        }
    }
}
