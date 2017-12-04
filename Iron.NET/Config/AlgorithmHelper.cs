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