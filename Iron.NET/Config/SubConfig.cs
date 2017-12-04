namespace Fantasista.IronDotNet.Config
{
    public class SubConfig
    {
        public int SaltBits {get;set;}
        public Algorithm Algorithm {get;set;}
        public int Iterations {get;set;}
        public int MinPasswordLength {get;set;}
        public string Salt {get;set;}
        public byte[] Iv { get; internal set; }
    }
}