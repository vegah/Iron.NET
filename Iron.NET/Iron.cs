/*
Copyright 2017 Vegard Berget

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using System;
using System.IO;
using System.Security.Cryptography;
using Fantasista.IronDotNet.Config;
using Fantasista.IronDotNet.Exceptions;

namespace Fantasista.IronDotNet
{
    public class Iron
    {
        private IronConfig _config;
        

        public string Digest { get; private set; }
        public string Salt { get; private set; }

        public Iron(IronConfig config ) 
        {
            _config = config;
        }

        public Iron() : this(IronConfig.Default)
        {

        }

        private HmacResult HmacWithPassword(string password, SubConfig options, string toHmac)
        {
            var key = GenerateKey(password,options);
            var hmac = new HMACSHA256(key.Key);
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(toHmac));
            var hashB64 = Util.Base64UrlEncode(hash);
            return new HmacResult()
            {
                Digest= hashB64,
                Salt= key.Salt
            };
        }

        public string Unseal(string sealedString, string password, IronConfig config)
        {
            var macBase = MacBase.FromSealedString(sealedString);
            if (!string.IsNullOrEmpty(macBase.Expiration))
            {
                CheckExpirationDate(macBase.Expiration,config);
            }
            var normalizedPassword = NormalizePassword(password);
            var decryptOptions = config.EncryptionConfig;
            decryptOptions.Salt = macBase.HmacSalt;
            var mac = HmacWithPassword(normalizedPassword.Integrity,decryptOptions,macBase.ToShortString());
            if (mac.Digest!=macBase.Hmac)
            {
                throw new IronUnsealErrorException("Bad HMAC value");
            }
            var encryptedUnBase64 = Util.Base64UrlDecode(macBase.EncryptedB64);
            decryptOptions.Iv = Util.Base64UrlDecode(macBase.EncryptionIv);
            decryptOptions.Salt = macBase.EncryptionSalt;
            var decrypted = Decrypt(normalizedPassword.Encryption,decryptOptions,encryptedUnBase64);
            return decrypted.DecryptedResult;
            
        }

        private static void CheckExpirationDate(string expiration,IronConfig config)
        {
            var _now = DateTime.Now.AddMilliseconds(config.LocalTimeOffsetMsec);
            var expiryDate = new DateTime(long.Parse(expiration));
            if (expiryDate <= _now)
            {
                throw new IronUnsealErrorException("Expired seal");
            }
        }

        public string Seal(string stringToSeal, string password, IronConfig options)
        {
            var date = DateTime.Now.AddMilliseconds(options.LocalTimeOffsetMsec);
            var normalizedPassword = NormalizePassword(password);
            var encrypted = Encrypt(password,options.EncryptionConfig,stringToSeal);
            var encryptedB64 = Util.Base64UrlEncode(encrypted.EncryptedResult);
            var iv = Util.Base64UrlEncode(encrypted.Key.Iv);
            var expiration = DateTime.Now.AddMilliseconds(options.Ttl);
            var hmacBase = MacBase.FromParameters("",encrypted.Key.Salt,iv,encryptedB64,"");
            var hmac = HmacWithPassword(password,options.IntegrityConfig,hmacBase.ToShortString());
            hmacBase.SetHmacSalt(hmac.Salt,hmac.Digest);
            return hmacBase.ToString();
        }

        private IronEncryptResult Encrypt(string password, SubConfig config, string toEncrypt)
        {
            var key = GenerateKey(password,config);
            using (var encryption = AlgorithmHelper.GetEncryption(config.Algorithm,key.Key,key.Iv))
            {
                return EncryptWithCorrectEncryption(toEncrypt, key, encryption);
            }
        }

        private static IronEncryptResult EncryptWithCorrectEncryption(string toEncrypt, IronKeyResult key, ICryptoTransform encryption)
        {
            using (var memoryStream = new MemoryStream())
            {
                return WriteEncryptionToMemoryStream(toEncrypt, key, encryption, memoryStream);
            }
        }

        private static IronEncryptResult WriteEncryptionToMemoryStream(string toEncrypt, IronKeyResult key, ICryptoTransform encryption, MemoryStream memoryStream)
        {
            using (var cryptoStream = new CryptoStream(memoryStream, encryption, CryptoStreamMode.Write))
            {
                return WriteContentToCryptoStreamAndReturnEncryptedResult(toEncrypt, key, memoryStream, cryptoStream);
            }
        }

        private static IronEncryptResult WriteContentToCryptoStreamAndReturnEncryptedResult(string toEncrypt, IronKeyResult key, MemoryStream memoryStream, CryptoStream cryptoStream)
        {
            var bytesToEncrypt = System.Text.Encoding.UTF8.GetBytes(toEncrypt);
            cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
            cryptoStream.FlushFinalBlock();
            var encrypted = memoryStream.ToArray();
            return new IronEncryptResult()
            {
                Key = key,
                EncryptedResult = encrypted
            };
        }

        private IronDecryptResult Decrypt(string password, SubConfig config, byte[] toDecrypt)
        {
            var key = GenerateKey(password,config);
            using (var encryption = AlgorithmHelper.GetDecryption(config.Algorithm,key.Key,key.Iv))
            {
                return DecryptWithCorrectDecryptionAlgorithm(toDecrypt, key, encryption);
            }
        }

        private static IronDecryptResult DecryptWithCorrectDecryptionAlgorithm(byte[] toDecrypt, IronKeyResult key, ICryptoTransform encryption)
        {
            using (var memoryStream = new MemoryStream())
            {
                return CreateCryptoStreamAndDecryptResult(toDecrypt, key, encryption, memoryStream);
            }
        }

        private static IronDecryptResult CreateCryptoStreamAndDecryptResult(byte[] toDecrypt, IronKeyResult key, ICryptoTransform encryption, MemoryStream memoryStream)
        {
            using (var cryptoStream = new CryptoStream(memoryStream, encryption, CryptoStreamMode.Write))
            {

                cryptoStream.Write(toDecrypt, 0, toDecrypt.Length);
                cryptoStream.FlushFinalBlock();
                var encrypted = memoryStream.ToArray();
                return new IronDecryptResult()
                {
                    Key = key,
                    DecryptedResult = System.Text.Encoding.UTF8.GetString(encrypted)
                };
            }
        }

        private IronKeyResult GenerateKey(string password,SubConfig config)
        {
            CheckConfiguration(password, config);
            string salt = CreateSalt(config);
            byte[] iv = CreateIvBytes(config);
            return GenerateKeyBasedOnSaltIvAndPassword(password, config, salt, iv);
        }

        private static IronKeyResult GenerateKeyBasedOnSaltIvAndPassword(string password, SubConfig config, string salt, byte[] iv)
        {
            var derivedBytes = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(password), System.Text.Encoding.UTF8.GetBytes(salt), config.Iterations);
            return new IronKeyResult()
            {
                Key = derivedBytes.GetBytes(32),
                Salt = salt,
                Iv = iv
            };
        }

        private static byte[] CreateIvBytes(SubConfig config)
        {
            var iv = config.Iv;
            if (iv == null)
            {
                iv = AlgorithmHelper.CreateIv(config.Algorithm);
            }

            return iv;
        }

        private static void CheckConfiguration(string password, SubConfig config)
        {
            if (string.IsNullOrEmpty(password))
                throw new IronMissingPasswordException("No password provided");
            if (password.Length < AlgorithmHelper.GetNumberOfBits(config.Algorithm) / 8)
                throw new IronConfigurationErrorException("Password buffer is too small");
        }

        private static string CreateSalt(SubConfig config)
        {
            var salt = config.Salt;
            if (string.IsNullOrEmpty(salt))
            {
                salt = Guid.NewGuid().ToString("N");
            }

            return salt;
        }


        private IronPassword NormalizePassword(string password)
        {
            return new IronPassword()
            {
                Encryption = password,
                Integrity = password
            };
        }
    }
}
