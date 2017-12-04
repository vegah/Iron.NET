using System;
using System.Security.Cryptography;

namespace Fantasista.IronDotNet.Config
{
    public class AlgorithmHelper
    {
        public static int GetNumberOfBits(Algorithm algorithm)
        {
            // Support only 256 bits now.
            return 256;
        }

        public static int GetNumberOfIvBits(Algorithm algorithm)
        {
            switch (algorithm)
            {
                case Algorithm.AES256cbc:
                    return 128;
                default:
                    return 0;
            }
        }

        public static byte[] CreateIv(Algorithm algorithm)
        {
            var aes = new AesCryptoServiceProvider();
            if (algorithm==Algorithm.AES256cbc)
            {
                 aes.GenerateIV();
                 return aes.IV;
            }
            return new byte[0];
        }

        internal static ICryptoTransform GetEncryption(Algorithm algorithm, byte[] key, byte[] iv)
        {
            if (algorithm==Algorithm.AES256cbc)
            {
                var crypto = new AesCryptoServiceProvider();
                return crypto.CreateEncryptor(key,iv);
            }
            return null;
        }

        internal static ICryptoTransform GetDecryption(Algorithm algorithm, byte[] key, byte[] iv)
        {
            if (algorithm==Algorithm.AES256cbc)
            {
                var crypto = new AesCryptoServiceProvider();
                return crypto.CreateDecryptor(key,iv);
            }
            return null;
        }
        
    }
}