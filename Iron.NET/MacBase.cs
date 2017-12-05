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
{   internal class MacBase
    {
        private const string MacFormatVersion = "2";
        private const string MacPrefix = "Fe26." + MacFormatVersion;
        private DateTime _now;
        private string _macPrefix;
        private string _passwordId;
        internal string EncryptionSalt {get; private set;}
        internal string EncryptionIv {get;private set;}
        internal string EncryptedB64 {get; private set;}
        internal string Expiration {get;private set;}
        internal string HmacSalt {get; private set;}
        internal string Hmac {get; private set;}
        private string _macBaseString;

        public MacBase()
        {

        }

        internal void SetHmacSalt(string salt, string digest)
        {
            Hmac = digest;
            HmacSalt = salt;
        }

        internal static  MacBase FromParameters(string passwordid,string encryptionsalt, string iv, string encryptedB64String, string expiration)
        {
            var macBase = new MacBase();
            macBase._passwordId=passwordid;
            macBase.EncryptionSalt=encryptionsalt;
            macBase.EncryptionIv=iv;
            macBase.EncryptedB64=encryptedB64String;
            macBase.Expiration=expiration;
            return macBase;
        }

        internal static MacBase FromSealedString(string sealedString)
        {
            var mac = new MacBase();
            
            var parts = sealedString.Split('*');
            if (parts.Length!=8)
                throw new IronUnsealErrorException("Sealed string must be 8 parts");
            mac._macPrefix = parts[0];
            mac._passwordId = parts[1];
            mac.EncryptionSalt = parts[2];
            mac.EncryptionIv = parts[3];
            mac.EncryptedB64 = parts[4];
            mac.Expiration = parts[5];
            mac.HmacSalt = parts[6];
            mac.Hmac = parts[7];
            if (mac._macPrefix!=MacPrefix)
                throw new IronUnsealErrorException("Mac prefix is wrong");
            return mac;
        }

        public string ToShortString()
        {
            return $"{MacPrefix}*{_passwordId}*{EncryptionSalt}*{EncryptionIv}*{EncryptedB64}*{Expiration}";
        }

        public override string ToString()
        {
            return ToShortString()+"*"+HmacSalt+"*"+Hmac;                
        }
    }
}