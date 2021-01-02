using System;
using System.Text;

namespace SCS.HomePhotos
{
    public static class StringExtensions
    {
        public static string Base64Encode(this string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string Base64Decode(this string base64Text)
        {
            var bytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Encrypt(this string text, string passcode)
        {
            return AesEncrypter.Encrypt(text, passcode).Base64Encode();
        }

        public static string Decrypt(this string text, string passcode)
        {
            return AesEncrypter.Decrypt(text.Base64Decode(), passcode);
        }
    }
}
