using CryptoNet;

using System;
using System.IO;
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
        /// <param name="key">The key to use.</param>
        /// <param name="passcode">The pass code to use.</param>
        /// <returns>An encrypted string.</returns>
        public static string Encrypt(this string text, string key, string passcode)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var iv = Encoding.UTF8.GetBytes(passcode);

            ICryptoNet encryptClient = new CryptoNetAes(keyBytes, iv);
            var encrypt = encryptClient.EncryptFromString(text);

            return Convert.ToBase64String(encrypt).Replace("+", "-").Replace("/", "_");
        }

        /// <summary>
        /// Decrypts the specified text.
        /// </summary>
        /// <param name="text">The text to decode.</param>
        /// <param name="key">The key to use.</param>
        /// <param name="passcode">The pass code to use.</param>
        /// <returns>The decrypted string.</returns>
        public static string Decrypt(this string text, string key, string passcode)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var iv = Encoding.UTF8.GetBytes(passcode);

            ICryptoNet decryptClient = new CryptoNetAes(keyBytes, iv);

            return decryptClient.DecryptToString(Convert.FromBase64String(text.Replace("-", "+").Replace("_", "/")));
        }

        /// <summary>
        /// Cleans a string to allow it to used as a file name.
        /// </summary>
        /// <param name="text">The text to clean.</param>
        /// <returns>The cleaned string.</returns>
        public static string CleanForFileName(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                text = text.Replace(c.ToString(), string.Empty);
            }
            return text;
        }
    } 
}
