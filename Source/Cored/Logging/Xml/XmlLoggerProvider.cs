namespace Cored.Logging.Xml
{
    using System.Collections.Concurrent;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    /// <summary>
    /// Provides the ability to create instances of <see cref="T:Microsoft.Extensions.Logging.ILogger" />
    /// </summary>
    public class XmlLoggerProvider : ILoggerProvider
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlLoggerProvider"/> class.
        /// </summary>
        /// <param name="path">The path of the file to log to</param>
        /// <param name="configuration">The configuration setting to use</param>
        public XmlLoggerProvider(string path, LoggerConfiguration configuration)
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
        /// Gets the configuration to use when creating a <see cref="XmlLogger"/>
        /// </summary>
        private readonly LoggerConfiguration _configuration;

        /// <summary>
        /// Keeps track of the loggers already created.
        /// </summary>
        private readonly ConcurrentDictionary<string, XmlLogger> _loggers = new ConcurrentDictionary<string, XmlLogger>();

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
            .GetOrAdd(categoryName, value => new XmlLogger(_filePath, _configuration));

        #endregion
    }
}