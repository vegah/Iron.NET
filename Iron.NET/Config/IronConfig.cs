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
namespace Fantasista.IronDotNet.Config
{
    public class IronConfig
    {
        public IronConfig()
        {
            EncryptionConfig = new SubConfig() {
                Algorithm = Algorithm.AES256cbc,
                SaltBits = 256,
                Iterations = 1,
                MinPasswordLength = 32
            };
            IntegrityConfig = new SubConfig() {
                Algorithm = Algorithm.Sha256,
                SaltBits = 256,
                Iterations = 1,
                MinPasswordLength = 32
            };
        }
        public SubConfig EncryptionConfig {get;private set;}
        public SubConfig IntegrityConfig  {get; private set;}
        public int Ttl {get;set;}
        public int TimeStampSkewSec {get;set;}
        public int LocalTimeOffsetMsec {get;set;}

        public static IronConfig Default {
        get{
            return new IronConfig() {
                Ttl = 0,
                TimeStampSkewSec = 60,
                LocalTimeOffsetMsec = 0
            };
            
        }
    }

    }

}