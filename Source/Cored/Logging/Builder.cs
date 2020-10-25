namespace Cored.Logging
{
    using File;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Extension methods for the <see cref="ILogger"/> implemented classes.
    /// </summary>
    public static class Builder
    {
        /// <summary>
        /// Adds a new file logger to the specific path.
        /// </summary>
        /// <param name="builder">The log builder to add to</param>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="configuration">The configuration to use</param>
        /// <returns></returns>
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string path,
            FileLoggerConfiguration configuration = null)
        {
            // Create default configuration if not provided
            configuration ??= new FileLoggerConfiguration();

            // Add file log provider to builder
            builder.AddProvider(new FileLoggerProvider(path, configuration));

            // Return the builder
            return builder;
        }

        // TODO: Add framework construction
    }
}