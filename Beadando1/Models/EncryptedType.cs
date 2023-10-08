using System.Security.Cryptography;
using System.Text;
using Utility;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Threading.Tasks;

namespace Beadando1.Models
{
    public class EncryptedType
    {

        //...
        //...Key  // user email
        //...Secret  // data to be encrypted
        //...
        public string Key { get; set; }
        public string Secret { get; set; }

        public EncryptedType(string key, string secret)
        {
            this.Key = key;
            this.Secret = secret;
        }

        public EncryptedType Encrypt()
        {
            using var hashing = SHA256.Create();
            byte[] keyHash = hashing.ComputeHash(Encoding.Unicode.GetBytes(Key));
            string key = Base64UrlEncoder.Encode(keyHash);
            string message = Base64UrlEncoder.Encode(Encoding.Unicode.GetBytes(Secret));
            return new(Key, Cryptography.Fernet.Encrypt(key, message));
        }

        public EncryptedType Decrypt()
        {
            using var hashing = SHA256.Create();
            byte[] keyHash = hashing.ComputeHash(Encoding.Unicode.GetBytes(Key));
            string key = Base64UrlEncoder.Encode(keyHash);
            string encodedSecret = Cryptography.Fernet.Decrypt(key, Secret);
            string message = Encoding.Unicode.GetString(Base64UrlEncoder.DecodeBytes(encodedSecret));
            return new(Key, message);
        }

        public string MainPasswordHash()
        {
            // fő jelszó generálása, nem a legszebb, hogy ide lett betéve szerintem, de jó így is mostra
            // SHA-512 hash generálása
            byte[] passwordBytes = Encoding.UTF8.GetBytes(Secret);
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashedBytes = sha512.ComputeHash(passwordBytes);
                string base64 = Convert.ToBase64String(hashedBytes);
                return base64;
            }
        }
    }
}
