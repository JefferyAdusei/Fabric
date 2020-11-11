namespace Cored.Logging.Xml
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.Extensions.Logging;
    using Async;

    /// <inheritdoc />
    /// <summary>
    /// A logger that writes logs to file
    /// </summary>
    public class XmlLogger : ILogger
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlLogger"/> class.
        /// </summary>
        /// <param name="filePath">The file path to write logs to</param>
        /// <param name="configuration">The configuration to use</param>
        public XmlLogger(string filePath, LoggerConfiguration configuration)
        {
            // Set members
            _filePath = Path.GetFullPath(filePath);
            _directory = Path.GetDirectoryName(_filePath);
            _configuration = configuration;
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets a unique key to lock files
        /// </summary>
        private static string FileLock => nameof(XmlLogger) + Guid.NewGuid();

        #endregion

        #region Private Members

        /// <summary>
        /// The file path to write log to.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The path to the directory the log file is in.
        /// </summary>
        private readonly string _directory;

        /// <summary>
        /// The log settings to use.
        /// </summary>
        private readonly LoggerConfiguration _configuration;

        /// <summary>
        /// The document model provided by LINQ to create xml documents
        /// </summary>
        private XDocument _xmlDocument;

        #endregion

        #region Implementation of ILogger

        /// <summary>
        /// Logs the message to file
        /// </summary>
        /// <typeparam name="TState">The type for the state</typeparam>
        /// <param name="logLevel">The log level</param>
        /// <param name="eventId">The event Id</param>
        /// <param name="state">The details of the message</param>
        /// <param name="exception">Any exception to add to the log</param>
        /// <param name="formatter">The formatter for converting the state and exception to a message string</param>
        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter = null)
        {
            // If we should not log...
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // Get values from the state
            object[] values = state as object[];

            await AsyncLock.LockAsync(FileLock, () =>
            {
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }

                // Create the xml file if it does not exist
                if (!File.Exists(_filePath))
                {
                    _xmlDocument = new XDocument(
                                                 new XDeclaration("1.0", "UTF-8", "yes"),
                                                 new XElement("Logs"));
                    _xmlDocument.Save(_filePath);
                }

                // Load the xml file
                _xmlDocument = XDocument.Load(_filePath);

                // Add new log to the logs
                _xmlDocument?.Element("Logs")?
                    .AddFirst(new XElement($"{logLevel.ToString()}",
                                           new XAttribute("Date", DateTime.Now.ToString("g")),
                                           new XAttribute("Filepath", $"{values?[1]}"),

                                           new XElement("Exception", new XAttribute("EventId", eventId), exception.Source),

                                           new XElement("Message",
                                                        new XAttribute("Origin", $"{values?[0]}"),
                                                        new XAttribute("LineNumber", $"{values?[2]}"),
                                                        $"{values?[3]}")
                                           )
                              );

                // Save the document
                _xmlDocument.Save(_filePath);

                return Task.FromResult(true);
            });
        }

        /// <inheritdoc />
        /// Enabled if the log level is the same or greater than the configuration
        public bool IsEnabled(LogLevel logLevel) => logLevel >= _configuration.LogLevel;

        /// <inheritdoc />
        /// File loggers are not scoped so this is always null
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        #endregion
    }
}