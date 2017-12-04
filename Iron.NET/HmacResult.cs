namespace Fantasista.IronDotNet
{
    public class HmacResult
    {
        public string Digest { get; internal set; }
        public string Salt { get; internal set; }
    }
}