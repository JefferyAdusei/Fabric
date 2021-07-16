namespace Cored.Logging.File
{
    using System.Collections.Concurrent;

    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    /// <summary>
    /// Provides the ability to create instances of <see cref="T:Microsoft.Extensions.Logging.ILogger" />
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoggerProvider"/> class.
        /// </summary>
        /// <param name="path">The path of the file to log to</param>
        /// <param name="configuration">The configuration setting to use</param>
        public FileLoggerProvider(string path, Configurator configuration)
        {
            // Set the configuration
            _configuration = configuration;

            // Set the path
            _filePath = path;
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Gets the path to the log file
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Gets the configuration to use when creating a <see cref="FileLogger"/>
        /// </summary>
        private readonly Configurator _configuration;

        /// <summary>
        /// Keeps track of the loggers already created.
        /// </summary>
        private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            // Clear the list of loggers
            _loggers.Clear();
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName) => _loggers
            .GetOrAdd(categoryName, _ => new FileLogger(_filePath, _configuration));

        #endregion
    }
}