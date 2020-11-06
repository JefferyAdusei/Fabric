namespace Cored.Logging
{
    using Microsoft.Extensions.Logging;
    using File;
    using Xml;

    /// <summary>
    /// Extension methods for the <see cref="ILogger"/> implemented classes.
    /// </summary>
    public static class Builder
    {
        #region File

        /// <summary>
        /// Adds a new file logger to the specified path.
        /// </summary>
        /// <param name="builder">The log builder to add to</param>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="configuration">The configuration to use</param>
        /// <returns></returns>
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string path,
            LoggerConfiguration configuration = null)
        {
            // Create default configuration if not provided
            configuration ??= new LoggerConfiguration();

            // Add file log provider to builder
            builder.AddProvider(new FileLoggerProvider(path, configuration));

            // Return the builder
            return builder;
        }

        #endregion

        #region Xml

        /// <summary>
        /// Adds a new xml logger to the specified path.
        /// </summary>
        /// <param name="builder">The log builder to add to</param>
        /// <param name="path">The path of the xml file to write to</param>
        /// <param name="configuration">The configuration to use</param>
        /// <returns></returns>
        public static ILoggingBuilder AddXml(this ILoggingBuilder builder, string path,
            LoggerConfiguration configuration = null)
        {
            // Create default configuration if not provided
            configuration ??= new LoggerConfiguration();

            // Add xml log provider to builder
            builder.AddProvider(new XmlLoggerProvider(path, configuration));

            // Return the builder
            return builder;
        }

        #endregion

        // TODO: Add framework construction
    }
}