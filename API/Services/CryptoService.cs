using System.Security.Cryptography;
using System.Text;

namespace API.Services
{
    public static class CryptoService
    {
        public static string Encrypt(string text, string key)
        {
            using Aes encryptor = Aes.Create();
            encryptor.KeySize = 128;
            encryptor.Key = Encoding.UTF8.GetBytes(key);
            encryptor.Mode = CipherMode.CBC;
            encryptor.Padding = PaddingMode.PKCS7;

            encryptor.GenerateIV();

            using MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] plaintextBytes = Encoding.UTF8.GetBytes(text);
                cs.Write(plaintextBytes, 0, plaintextBytes.Length);
            }

            byte[] ivBytes = encryptor.IV;
            byte[] encryptedBytes = ms.ToArray();
            byte[] resultBytes = new byte[ivBytes.Length + encryptedBytes.Length];
            Array.Copy(ivBytes, resultBytes, ivBytes.Length);
            Array.Copy(encryptedBytes, 0, resultBytes, ivBytes.Length, encryptedBytes.Length);

            return Convert.ToBase64String(resultBytes);
        }

        public static string Decrypt(string encryptedText, string key)
        {
            byte[] inputBytes = Convert.FromBase64String(encryptedText);

            using Aes decryptor = Aes.Create();
            decryptor.KeySize = 128;
            decryptor.Key = Encoding.UTF8.GetBytes(key);
            decryptor.Mode = CipherMode.CBC;
            decryptor.Padding = PaddingMode.PKCS7;

            byte[] ivBytes = new byte[decryptor.BlockSize / 8];
            byte[] encryptedBytes = new byte[inputBytes.Length - ivBytes.Length];
            Array.Copy(inputBytes, ivBytes, ivBytes.Length);
            Array.Copy(inputBytes, ivBytes.Length, encryptedBytes, 0, encryptedBytes.Length);

            decryptor.IV = ivBytes;

            using MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, decryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(encryptedBytes, 0, encryptedBytes.Length);
            }

            byte[] decryptedBytes = ms.ToArray();
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
