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