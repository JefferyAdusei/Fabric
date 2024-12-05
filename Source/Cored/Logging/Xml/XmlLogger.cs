namespace Cored.Logging.Xml
{
    using Async;
    using Cored.Logging.Model;
    using Microsoft.Extensions.Logging;
    using Reflection;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <inheritdoc />
    /// <summary>
    /// A logger that writes logs to file
    /// </summary>
    public class XmlLogger : ILogger
    {
        #region Private Members

        /// <summary>
        /// The path to the directory the log file is in.
        /// </summary>
        private readonly string _directory;

        /// <summary>
        /// The log settings to use.
        /// </summary>
        private readonly Configurator _configurator;

        /// <summary>
        /// The document model provided by LINQ to create xml documents
        /// </summary>
        private XDocument _xmlDocument;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlLogger"/> class.
        /// </summary>
        /// <param name="configurator">The configuration to use</param>
        public XmlLogger(Configurator configurator)
        {
            // Set members
            _directory = Path.GetDirectoryName(configurator.FilePath);
            _configurator = configurator;
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets a unique key to lock files
        /// </summary>
        private static string FileLock => nameof(XmlLogger) + Guid.NewGuid();

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
        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // If we should not log...
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // If message is from source, then set the message values
            // Other wise use the generic ones provides and log only the message
            object[] values = state as object[] ?? new object[] { $"{Path.GetFileName(typeof(TState).FileLocation())}", $"{Directory.GetCurrentDirectory()}", "0", $"{state}" };

            await AsyncLock.LockAsync(FileLock, async () =>
            {
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }

                // Initialize the log
                Log log = new Log()
                {
                    Details = new Details
                    {
                        LineNumber = $"{values[2]}",
                        Message = $"{values[3]}",
                        Origin = $"{values[0]}"
                    },
                    Date = $"{DateTime.Now:g}",
                    Exception = exception.Source,
                    FilePath = $"{values[1]}",
                    LogLevel = logLevel
                };

                // Serialize the xml log
                XmlSerializer xml = new XmlSerializer(typeof(Log));

                // Open the file
                await using StreamWriter xmlStream = 
                    new(File.Open(_configurator.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                    FileShare.ReadWrite));

                xmlStream.BaseStream.Seek(0, SeekOrigin.End);

                xml.Serialize(xmlStream, log);

                // Flush the stream
                await xmlStream.FlushAsync();

                // Return from the async lock
                return Task.FromResult(true);
            });
        }

        /// <inheritdoc />
        /// Enabled if the log level is the same or greater than the configuration
        public bool IsEnabled(LogLevel logLevel) => logLevel >= _configurator.LogLevel;

        /// <inheritdoc />
        /// File loggers are not scoped so this is always null
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        #endregion

        #region Obsolete Implementation

        /// <summary>
        /// Logs the message to file
        /// </summary>
        /// <typeparam name="TState">The type for the state</typeparam>
        /// <param name="logLevel">The log level</param>
        /// <param name="eventId">The event Id</param>
        /// <param name="state">The details of the message</param>
        /// <param name="exception">Any exception to add to the log</param>
        [Obsolete]
        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception)
        {
            // If we should not log...
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // If message is from source, the set the message values
            // Other wise use the generic ones provides and log only the message
            object[] values = state as object[] ?? new object[] { $"{Path.GetFileName(typeof(TState).FileLocation())}", $"{Directory.GetCurrentDirectory()}", "0", $"{state}" };

            await AsyncLock.LockAsync(FileLock, async () =>
            {
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }

                // Create the xml file if it does not exist
                if (!File.Exists(_configurator.FilePath))
                {
                    _xmlDocument = new XDocument(
                                                 new XDeclaration("1.0", "UTF-8", "yes"),
                                                 new XElement("Logs"));
                    _xmlDocument.Save(_configurator.FilePath);
                }

                // Load the xml file
                _xmlDocument = XDocument.Load(_configurator.FilePath);

                // Add new log to the logs
                _xmlDocument?.Element("Logs")?
                    .AddFirst(new XElement($"{logLevel}",
                                           new XAttribute("Date", DateTime.Now.ToString("g")),
                                           new XAttribute("Filepath", $"{values[1]}"),

                                           new XElement("Exception", new XAttribute("EventId", eventId), exception?.Source),

                                           new XElement("Message",
                                                        new XAttribute("Origin", $"{values[0]}"),
                                                        new XAttribute("LineNumber", $"{values[2]}"),
                                                        $"{values[3]}")
                                          )
                             );

                // Save the document
                _xmlDocument.Save(_configurator.FilePath);

                return await Task.FromResult(true);
            });
        }


        #endregion
    }
}