﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace UniSA_Radiation_Therapy_Mock_Clinic_Scheduler.Firebase
{
    public class FirebaseSymmetricEncryption
    {
        private string encryptionKey;

        public FirebaseSymmetricEncryption(string encryptionKey)
        {
            this.encryptionKey = encryptionKey;
        }

        public string SymmetricEnctyption(string toEncrypt)
        {
            byte[] bytes = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
                aes.IV = bytes;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(toEncrypt);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
    
            return Convert.ToBase64String(array);
        }

        public string SymmetricDecryption(string toDecrypt)
        {
            byte[] bytes = new byte[16];
            byte[] buffer = Convert.FromBase64String(toDecrypt);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
                aes.IV = bytes;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

