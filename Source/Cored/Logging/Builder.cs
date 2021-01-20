namespace Cored.Logging
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Fabric.Construction;
    using File;
    using Xml;

    /// <summary>
    /// Extension methods for the <see cref="ILoggingBuilder"/> implemented classes.
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

        #region Framework Construction

        /// <summary>
        /// Injects a file logger into the fabric construction
        /// </summary>
        /// <param name="construction">The calling fabric construction to be chained</param>
        /// <param name="logPath">The path to the log file</param>
        /// <returns>The fabric construction for chaining</returns>
        public static FabricConstruction AddFileLogger(this FabricConstruction construction, string logPath = "log.txt")
        {
            // Make use of default AddLogging extension that comes with .NET Core's dependency injection
            construction.ServiceCollection.AddLogging(options => options?.AddFile(logPath));

            // Chain the construction
            return construction;
        }

        /// <summary>
        /// Injects an xml logger into the fabric construction
        /// </summary>
        /// <param name="construction">The calling fabric construction to be chained</param>
        /// <param name="logPath">The path to the log file</param>
        /// <returns>The fabric construction for chaining</returns>
        public static FabricConstruction AddXmlLogger(this FabricConstruction construction, string logPath = "log.xml")
        {
            // Make use of default AddLogging extension that comes with .NET Core's dependency injection
            construction.ServiceCollection.AddLogging(builder => builder?.AddXml(logPath));

            // Chain the construction
            return construction;
        }

        #endregion
    }
}