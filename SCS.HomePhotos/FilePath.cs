using System;
using System.IO;

namespace SCS.HomePhotos
{
    /// <summary>
    /// A file path helper.
    /// </summary>
    public static class FilePath
    {
        /// <summary>
        /// Combines file path segments into one full path.
        /// </summary>
        /// <param name="parts">The file segments.</param>
        /// <returns>A full file or directory path.</returns>
        /// <exception cref="System.ArgumentException">One or more segments are required. - parts</exception>
        public static string Combine(params string[] parts)
        {
            if (parts == null || parts.Length == 0)
            {
                throw new ArgumentException("One or more segments are required.", nameof(parts));
            }

            var newParts = new string[parts.Length];

            for (var i = 0; i < parts.Length; i++)
            {
                newParts[i] = EnsureCorrectPathSeparator(parts[i]).Trim(Path.DirectorySeparatorChar);
            }

            return Path.Combine(newParts);
        }

        /// <summary>
        /// Ensures the correct path separator is being used in a file/directory path.
        /// </summary>
        /// <param name="path">The file or directory path.</param>
        /// <returns>The updated path.</returns>
        public static string EnsureCorrectPathSeparator(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Formats a file or directory path to correct relative path format.
        /// </summary>
        /// <param name="path">The file or directory path.</param>
        /// <returns>The updated path.</returns>
        public static string FormatRelativePath(string path)
        {
            path = EnsureCorrectPathSeparator(path);
            return Path.DirectorySeparatorChar + path.Trim(Path.DirectorySeparatorChar);
        }
    }
}
