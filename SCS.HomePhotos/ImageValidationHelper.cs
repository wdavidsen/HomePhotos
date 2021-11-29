using System.Collections.Generic;
using System.IO;

namespace SCS.HomePhotos
{
    /// <summary>
    /// An image validation helper.
    /// </summary>
    public static class ImageValidationHelper
    {
        private static readonly Dictionary<string, string[]> _jpegBytes;
        private static readonly Dictionary<string, string[]> _pngBytes;
        private static readonly Dictionary<string, string[]> _gifBytes;
        
        static ImageValidationHelper()
        {
            _jpegBytes = new Dictionary<string, string[]>
            {
                { "Type1", new string[] { "FF", "D8", "FF", "DB" } },
                { "Type2", new string[] { "FF", "D8", "FF", "E0" } },
                { "Type3", new string[] { "FF", "D8", "FF", "EE" } },
                { "Type4", new string[] { "FF", "D8", "FF", "E1" } },
                { "Type5", new string[] { "FF", "D8", "FF", "E2" } },
                { "Type6", new string[] { "FF", "D8", "FF", "E3" } },
            };

            _pngBytes = new Dictionary<string, string[]>
            {
                { "Type1", new string[] { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" } }
            };

            _gifBytes = new Dictionary<string, string[]>
            {
                { "Type1", new string[] { "47", "49", "46", "38", "37", "61" } },
                { "Type2", new string[] { "47", "49", "46", "38", "39", "61" } },
            };
        }

        /// <summary>
        /// Verifies a file has the PNG header.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns>True or false.</returns>
        public static bool ValidatePngHeader(string sourcePath)
        {
            return ValidateImageHeader(sourcePath, _pngBytes["Type1"]);
        }

        /// <summary>
        /// Verifies a file has the JPG header.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns>True or false.</returns>
        public static bool ValidateJpegHeader(string sourcePath)
        {
            return ValidateImageHeader(sourcePath, _jpegBytes["Type1"])
                || ValidateImageHeader(sourcePath, _jpegBytes["Type2"])
                || ValidateImageHeader(sourcePath, _jpegBytes["Type3"])
                || ValidateImageHeader(sourcePath, _jpegBytes["Type4"]);
        }

        /// <summary>
        /// Verifies a file has the GIF header.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns>True or false.</returns>
        public static bool ValidateGifHeader(string sourcePath)
        {
            return ValidateImageHeader(sourcePath, _gifBytes["Type1"]) || ValidateImageHeader(sourcePath, _gifBytes["Type2"]);
        }

        /// <summary>
        /// Verifies a file has the provide byte codes in its header.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="hexCodes">The hex codes to look for.</param>
        /// <returns>True or false.</returns>
        public static bool ValidateImageHeader(string sourcePath, params string[] hexCodes)
        {
            using (var stream = File.OpenRead(sourcePath))
            {
                var data = new byte[8];
                int numRead = stream.Read(data, 0, data.Length);

                return CheckBytes(data, hexCodes);
            }
        }

        /// <summary>
        /// Validates and image steam and checks that is has either a JPG or PNG header.
        /// </summary>
        /// <param name="sourceStream">The source stream.</param>
        /// <returns>True or false.</returns>
        public static bool ValidateImageHeaders(Stream sourceStream)
        {
            var data = new byte[8];
            sourceStream.Read(data, 0, data.Length);

            return (CheckBytes(data, _jpegBytes["Type1"])
                    || CheckBytes(data, _jpegBytes["Type2"])
                    || CheckBytes(data, _jpegBytes["Type3"])
                    || CheckBytes(data, _jpegBytes["Type4"])) // JPG
                || CheckBytes(data, _pngBytes["Type1"]) // PNG
                || (CheckBytes(data, _gifBytes["Type1"])
                    || CheckBytes(data, _gifBytes["Type2"])); // GIF
        }

        /// <summary>
        /// Checks that the bytes start with provided hex codes.
        /// </summary>
        /// <param name="target">The target bytes.</param>
        /// <param name="hexCodes">The hexadecimal codes.</param>
        /// <returns>True or false.</returns>
        public static bool CheckBytes(byte[] target, params string[] hexCodes)
        {
            var index = 0;
            foreach (var code in hexCodes)
            {
                if (target[index].ToString("X").PadLeft(2, '0') != code)
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        /// <summary>
        /// Get extension of an image file.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns>String values ".jpeg", ".png", ".gif", or null.</returns>
        public static string GetExtension(string sourcePath)
        {
            if (ValidateJpegHeader(sourcePath))
            {
                return ".jpeg";
            }
            else if (ValidatePngHeader(sourcePath))
            {
                return ".png";
            }
            else if (ValidateGifHeader(sourcePath))
            {
                return ".gif";
            }

            return null;
        }
    }
}
