using System;
using System.Text;

namespace SCS.HomePhotos
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Base 64 encodes a string.
        /// </summary>
        /// <param name="text">The text to encode.</param>
        /// <returns>A encoded string.</returns>
        public static string Base64Encode(this string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decodes a base64 encoded string.
        /// </summary>
        /// <param name="base64Text">The base64 coded text.</param>
        /// <returns>The decoded string</returns>
        public static string Base64Decode(this string base64Text)
        {
            var bytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Encrypts the specified text.
        /// </summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="passcode">The passcode to use.</param>
        /// <returns>An encripted string.</returns>
        public static string Encrypt(this string text, string passcode)
        {
            return AesEncrypter.Encrypt(text, passcode).Base64Encode();
        }

        /// <summary>
        /// Decrypts the specified text.
        /// </summary>
        /// <param name="text">The text to decode.</param>
        /// <param name="passcode">The passcode to use.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(this string text, string passcode)
        {
            return AesEncrypter.Decrypt(text.Base64Decode(), passcode);
        }
    }
}
