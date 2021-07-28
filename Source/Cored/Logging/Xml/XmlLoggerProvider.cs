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
        /// <param name="configuration">The configuration setting to use</param>
        public XmlLoggerProvider(Configurator configuration)
        {
            // Set the configuration
            _configuration = configuration;
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// Gets the configuration to use when creating a <see cref="XmlLogger"/>
        /// </summary>
        private readonly Configurator _configuration;

        /// <summary>
        /// Keeps track of the loggers already created.
        /// </summary>
        private readonly ConcurrentDictionary<string, XmlLogger> _loggers = new();

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
            .GetOrAdd(categoryName, _ => new XmlLogger(_configuration));

        #endregion
    }
}