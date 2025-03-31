using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Supply_Box_Core
{
    public class EncryptionService
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("your-32-char-secret-key-here!"); // 32 bytes
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("your-16-char-IV-here!"); // 16 bytes

        // Método de criptografia agora inclui AppName
        public static string Encrypt(string site, string username, string password, string appName)
        {
            string plainText = $"{site}|{username}|{password}|{appName}";

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encrypted);
                }
            }
        }

        // Método de descriptografia agora retorna AppName também
        public static (string site, string username, string password, string appName) Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] decrypted = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    string decryptedText = Encoding.UTF8.GetString(decrypted);

                    // Separando os valores corretamente
                    string[] parts = decryptedText.Split('|');
                    if (parts.Length == 4)
                    {
                        return (parts[0], parts[1], parts[2], parts[3]); // Retorna todos os valores
                    }
                    throw new Exception("Dados corrompidos ou formato inválido.");
                }
            }
        }
    }
}