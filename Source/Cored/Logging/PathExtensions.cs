namespace Cored.Logging
{
    using Fabric.Di;
    using System.IO;

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
        public static string NormalizePath(this string path) => FabricDi.FabricEnvironment.IsWindows
            ? path?.Replace('/', '\\').Trim()
            : path?.Replace('\\', '/').Trim();

        /// <summary>
        /// Resolves any relative elements of the path to absolute
        /// </summary>
        /// <param name="path">The path to resolve</param>
        /// <returns>The resolved path</returns>
        public static string ResolvePath(this string path) => Path.GetFullPath(path);
    }
}