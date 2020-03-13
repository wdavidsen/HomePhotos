using System.IO;

namespace SCS.HomePhotos
{
    /// <summary>
    /// An image validation helper.
    /// </summary>
    public static class ImageValidationHelper
    {
        /// <summary>
        /// Verifies a file has the PNG header.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns>True or false.</returns>
        public static bool ValidatePngHeader(string sourcePath)
        {
            return ValidateImageHeader(sourcePath, "89", "50", "4E", "47");
        }

        /// <summary>
        /// Verifies a file has the JPG header.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns>True or false.</returns>
        public static bool ValidateJpegHeader(string sourcePath)
        {
            return ValidateImageHeader(sourcePath, "FF", "D8", "FF", "E0");
        }

        /// <summary>
        /// Verifies a file has the GIF header.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <returns>True or false.</returns>
        public static bool ValidateGifHeader(string sourcePath)
        {
            return ValidateImageHeader(sourcePath, "47", "49", "46");
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

            // JPG / PNG / GIF
            return CheckBytes(data, "FF", "D8", "FF", "E0") || CheckBytes(data, "89", "50", "4E", "47") || CheckBytes(data, "47", "49", "46");
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
                if (target[index].ToString("X") != code)
                {
                    return false;
                }
                index++;
            }
            return true;
        }
    }
}
