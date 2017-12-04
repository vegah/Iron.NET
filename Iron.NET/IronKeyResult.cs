namespace Fantasista.IronDotNet
{
    public class IronKeyResult
    {
        public byte[] Key { get; internal set; }
        public string Salt { get; internal set; }
        public byte[] Iv { get; internal set; }
    }
}