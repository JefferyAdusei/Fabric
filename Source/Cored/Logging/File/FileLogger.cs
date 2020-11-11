namespace Cored.Logging.File
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Async;

    /// <inheritdoc />
    /// <summary>
    /// A logger that writes logs as normal text to file
    /// </summary>
    public class FileLogger : ILogger
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="filePath">The file path to write logs to</param>
        /// <param name="configuration">The configuration to use</param>
        public FileLogger(string filePath, LoggerConfiguration configuration)
        {
            // Set members
            _filePath = Path.GetFullPath(filePath);
            _directory = Path.GetDirectoryName(_filePath);
            _configuration = configuration;
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets a unique key to lock file access
        /// </summary>
        private static string FileLock => nameof(FileLogger) + Guid.NewGuid();

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

            // Lock the file
            await AsyncLock.LockAsync(FileLock, async () =>
            {
                // Ensure folder exists
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }

                // Open the file
                await using StreamWriter fileStream =
                    new StreamWriter(File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                                               FileShare.ReadWrite));
                fileStream.BaseStream.Seek(0, SeekOrigin.End);

                // Write the message to the file
                await fileStream.WriteAsync($"{logLevel}: {DateTimeOffset.Now:g} {formatter(state, exception)} \n");

                return await Task.FromResult(true);
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