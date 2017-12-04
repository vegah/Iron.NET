using System;

namespace Fantasista.IronDotNet
{
    public class Util
    {
        public static string Base64UrlEncode(byte[] input) {
    
    // Special "url-safe" base64 encode.
    return Convert.ToBase64String(input)
      .Replace('+', '-')
      .Replace('/', '_')
      .Replace("=", "");
  }

        internal static byte[] Base64UrlDecode(string encryptedB64)
        {
            string s = encryptedB64;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default: throw new System.Exception(
                "Illegal base64url string!");
            }
            return Convert.FromBase64String(s);
        }
    }
}