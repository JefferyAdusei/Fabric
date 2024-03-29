﻿namespace Cored.Logging
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
        /// <param name="configuration">The configuration to use</param>
        /// <returns></returns>
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder,
            Configurator configuration = null)
        {
            // Create default configuration if not provided
            configuration ??= new Configurator();

            // Add file log provider to builder
            builder.AddProvider(new FileLoggerProvider(configuration));

            // Return the builder
            return builder;
        }

        #endregion

        #region Xml

        /// <summary>
        /// Adds a new xml logger to the specified path.
        /// </summary>
        /// <param name="builder">The log builder to add to</param>
        /// <param name="configuration">The configuration to use</param>
        /// <returns></returns>
        public static ILoggingBuilder AddXml(this ILoggingBuilder builder,
            Configurator configuration = null)
        {
            // Create default configuration if not provided
            configuration ??= new Configurator();

            // Add xml log provider to builder
            builder.AddProvider(new XmlLoggerProvider(configuration));

            // Return the builder
            return builder;
        }

        #endregion

        #region Framework Construction

        /// <summary>
        /// Injects a file logger into the fabric construction
        /// </summary>
        /// <param name="construction">The calling fabric construction to be chained</param>
        /// <param name="configurator">The configuration setting for the file logger, null to use default configuration</param>
        /// <returns>The fabric construction for chaining</returns>
        public static FabricConstruction AddFileLogger(this FabricConstruction construction, Configurator configurator = null)
        {
            // Make use of default AddLogging extension that comes with .NET Core's dependency injection
            construction.ServiceCollection.AddLogging(options => options?.AddFile(configurator));

            // Chain the construction
            return construction;
        }

        /// <summary>
        /// Injects an xml logger into the fabric construction
        /// </summary>
        /// <param name="construction">The calling fabric construction to be chained</param>
        /// <param name="configurator">The configuration setting for the file logger, null to use default configuration</param>
        /// <returns>The fabric construction for chaining</returns>
        public static FabricConstruction AddXmlLogger(this FabricConstruction construction, Configurator configurator = null)
        {
            // Make use of default AddLogging extension that comes with .NET Core's dependency injection
            construction.ServiceCollection.AddLogging(builder => builder?.AddXml(configurator));

            // Chain the construction
            return construction;
        }

        #endregion
    }
}