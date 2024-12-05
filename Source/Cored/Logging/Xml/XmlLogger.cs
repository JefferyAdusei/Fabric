using System.Xml.Serialization;

namespace Cored.Logging.Xml
{
    using Async;
    using Microsoft.Extensions.Logging;
    using Reflection;
    using System;
    using System.IO;
    using System.Threading.Tasks;

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
        /// The path to the log file
        /// </summary>
        private readonly string _filepath;

        /// <summary>
        /// The log settings to use.
        /// </summary>
        private readonly Configurator _configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlLogger"/> class.
        /// </summary>
        /// <param name="configuration">The configuration to use</param>
        public XmlLogger(Configurator configuration)
        {
            // Set members
            _directory = Path.GetDirectoryName(configuration.Path);
            _filepath = Path.Combine(_directory!,
                Path.GetFileName(configuration.FilePath).PathRoll(configuration.Roll));
            _configuration = configuration;
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
        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            // If we should not log...
            if (!IsEnabled(logLevel))
            {
                return;
            }

            // If message is from source, then set the message values
            // Other wise use the generic ones provides and log only the message
            object[] values = state as object[] ??
            [
                $"{Path.GetFileName(typeof(TState).FileLocation())}", $"{Directory.GetCurrentDirectory()}", "0",
                $"{state}"
            ];

            await AsyncLock.LockAsync(FileLock, async () =>
            {
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }

                // Initialize the log
                var log = new Log()
                {
                    Exception = exception?.Source,
                    FilePath = $"{values[1]}",
                    LogLevel = logLevel,
                    Message = new Message()
                    {
                        Line = (int) values[2],
                        Origin = $"{values[0]}",
                        Text = $"{values[3]}"
                    }
                };

                // Serialize the xml log
                XmlSerializer xml = new XmlSerializer(typeof(Log));

                // Open the file
                await using StreamWriter xmlStream =
                    new(File.Open(_filepath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
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