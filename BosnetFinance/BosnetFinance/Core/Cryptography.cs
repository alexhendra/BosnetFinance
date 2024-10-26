using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BosnetFinance.Core
{
    public class Cryptography
    {
        public static Dictionary<string, string> Encrypt(string plainText)
        {
            Dictionary<string, string> result = null;
            if (string.IsNullOrEmpty(plainText))
                return result;

            result = new Dictionary<string, string>();

            using (RijndaelManaged algo = new RijndaelManaged())
            {
                algo.GenerateKey();
                algo.GenerateIV();
                result.Add("Key", Convert.ToBase64String(algo.Key));
                result.Add("IV", Convert.ToBase64String(algo.IV));

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = algo.CreateEncryptor(algo.Key, algo.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        result.Add("EncrypText", Convert.ToBase64String(msEncrypt.ToArray()));
                    }
                }
            }
            return result;
        }

        public static string Encrypt(string plainText, string Key, string IV)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(IV))
                return result;

            using (RijndaelManaged algo = new RijndaelManaged())
            {
                algo.Key = Convert.FromBase64String(Key);
                algo.IV = Convert.FromBase64String(IV);

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = algo.CreateEncryptor(algo.Key, algo.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        result = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            return result;
        }

        public static string Decrypt(string cipherText, string Key, string IV)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(IV))
                return result;

            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Convert.FromBase64String(Key);
                rijAlg.IV = Convert.FromBase64String(IV);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return result;
        }
    }
}