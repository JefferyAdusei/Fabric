namespace Cored.Logging
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Extension method that normalizes file path based on the current OS Platform
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// Normalizes a path based on the current operating system
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>The normalized path</returns>
        public static string NormalizePath(this string path) => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? path?.Replace('/', '\\').Trim()
            : path?.Replace('\\', '/').Trim();

        /// <summary>
        /// Resolves any relative elements of the path to absolute
        /// </summary>
        /// <param name="path">The path to resolve</param>
        /// <returns>The resolved path</returns>
        public static string ResolvePath(this string path) => Path.GetFullPath(path);

        /// <summary>
        /// Generates a file name for logs depending on the user's roll over choice
        /// </summary>
        /// <param name="filePath">The file name to attach to roll over name</param>
        /// <param name="roll">The roll over choice of the user</param>
        /// <returns>The generated file name based of the roll over choice</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws an out of range exception</exception>
        public static string PathRoll(this string filePath, Roll? roll)
        {
            var today = DateTime.Today;

            return roll switch
            {
                Roll.Daily => $"[{today.Day}][{today.Month}][{today.Year}]{filePath}",
                Roll.Weekly => $"[{CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(today, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}][{today.Year}]{filePath}",
                Roll.Monthly => $"[{today.Month}][{today.Year}]{filePath}",
                Roll.Yearly => $"[{today.Year}]{filePath}",
                _ => filePath
            };
        }
    }
}