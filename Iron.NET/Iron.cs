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
        
        private const string MacFormatVersion = "2";
        private const string MacPrefix = "Fe26." + MacFormatVersion;

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
            var now = DateTime.Now.AddMilliseconds(config.LocalTimeOffsetMsec);
            var parts = sealedString.Split('*');
            if (parts.Length!=8)
                throw new IronUnsealErrorException("Sealed string must be 8 parts");
            var macPrefix = parts[0];
            var passwordId = parts[1];
            var encryptionSalt = parts[2];
            var encryptionIv = parts[3];
            var encryptedB64 = parts[4];
            var expiration = parts[5];
            var hmacSalt = parts[6];
            var hmac = parts[7];
            var macBaseString = macPrefix + '*' + passwordId + '*' + encryptionSalt + '*' + encryptionIv + '*' + encryptedB64 + '*' + expiration;
            if (macPrefix!=MacPrefix)
                throw new IronUnsealErrorException("Mac prefix is wrong");
            if (!string.IsNullOrEmpty(expiration))
            {
                var expiryDate = new DateTime(long.Parse(expiration));
                if (expiryDate<=DateTime.Now)
                {
                    throw new IronUnsealErrorException("Expired seal");
                }
            }
            var normalizedPassword = NormalizePassword(password);
            var decryptOptions = config.EncryptionConfig;
            decryptOptions.Salt = hmacSalt;
            var mac = HmacWithPassword(normalizedPassword.Integrity,decryptOptions,macBaseString);
            if (mac.Digest!=hmac)
            {
                throw new IronUnsealErrorException("Bad HMAC value");
            }
            var encryptedUnBase64 = Util.Base64UrlDecode(encryptedB64);
            decryptOptions.Iv = Util.Base64UrlDecode(encryptionIv);
            decryptOptions.Salt = encryptionSalt;
            var decrypted = Decrypt(normalizedPassword.Encryption,decryptOptions,encryptedUnBase64);
            return decrypted.DecryptedResult;
            
        }

        public string Seal(string stringToSeal, string password, IronConfig options)
        {
            var date = DateTime.Now.AddMilliseconds(options.LocalTimeOffsetMsec);
            var normalizedPassword = NormalizePassword(password);
            var encrypted = Encrypt(password,options.EncryptionConfig,stringToSeal);
            var encryptedB64 = Util.Base64UrlEncode(encrypted.EncryptedResult);
            var iv = Util.Base64UrlEncode(encrypted.Key.Iv);
            var expiration = DateTime.Now.AddMilliseconds(options.Ttl);
            var macBaseString = MacPrefix+"**"+encrypted.Key.Salt+"*"+iv+"*"+encryptedB64+"*";
            var hmac = HmacWithPassword(password,options.IntegrityConfig,macBaseString);
            return macBaseString+"*"+hmac.Salt+"*"+hmac.Digest;
        }

        private IronEncryptResult Encrypt(string password, SubConfig config, string toEncrypt)
        {
            var key = GenerateKey(password,config);
            using (var encryption = AlgorithmHelper.GetEncryption(config.Algorithm,key.Key,key.Iv))
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream,encryption,CryptoStreamMode.Write))
                    {
                        var bytesToEncrypt = System.Text.Encoding.UTF8.GetBytes(toEncrypt);
                        cryptoStream.Write(bytesToEncrypt,0,bytesToEncrypt.Length);
                        cryptoStream.FlushFinalBlock();
                        var encrypted = memoryStream.ToArray();
                        return new IronEncryptResult() {
                            Key = key,
                            EncryptedResult = encrypted
                        };
                    }
                }
            }
        }

        private IronDecryptResult Decrypt(string password, SubConfig config, byte[] toDecrypt)
        {
            var key = GenerateKey(password,config);
            using (var encryption = AlgorithmHelper.GetDecryption(config.Algorithm,key.Key,key.Iv))
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream,encryption,CryptoStreamMode.Write))
                    {
                        
                        cryptoStream.Write(toDecrypt,0,toDecrypt.Length);
                        cryptoStream.FlushFinalBlock();
                        var encrypted = memoryStream.ToArray();
                        return new IronDecryptResult() {
                            Key = key,
                            DecryptedResult = System.Text.Encoding.UTF8.GetString(encrypted)
                        };
                    }
                }
            }
        }


        private IronKeyResult GenerateKey(string password,SubConfig config)
        {
            if (string.IsNullOrEmpty(password)) 
                throw new IronMissingPasswordException("No password provided");
            if (password.Length<AlgorithmHelper.GetNumberOfBits(config.Algorithm)/8)
                throw new IronConfigurationErrorException("Password buffer is too small");
            var numberOfIvBits = AlgorithmHelper.GetNumberOfIvBits(config.Algorithm);
            var salt = config.Salt;
            if (string.IsNullOrEmpty(salt))
            {
                salt = Guid.NewGuid().ToString("N");
            }
            var iv = config.Iv;
            if (iv==null)
            {
                iv = AlgorithmHelper.CreateIv(config.Algorithm);
            }
            
            var derivedBytes = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(password), System.Text.Encoding.UTF8.GetBytes(salt),config.Iterations);

            return new IronKeyResult()
            {
                Key = derivedBytes.GetBytes(32),
                Salt =salt,
                Iv = iv
            };
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
