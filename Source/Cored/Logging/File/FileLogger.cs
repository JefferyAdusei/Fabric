namespace Cored.Logging.File
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    /// <summary>
    /// A logger that writes logs to file
    /// </summary>
    public class FileLogger : ILogger
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="filePath">The file path to write logs to</param>
        /// <param name="configuration">The configuration to use</param>
        public FileLogger(string filePath, FileLoggerConfiguration configuration)
        {
            // Get absolute path
            filePath = Path.GetFullPath(filePath);

            // Set members
            _filePath = filePath;
            _directory = Path.GetDirectoryName(filePath);
            _configuration = configuration;
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// A list of file locks based on file path
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> FileLocks = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// The lock to lock the list of locks
        /// </summary>
        private static readonly object FileLockLock = new object();

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
        private readonly FileLoggerConfiguration _configuration;

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
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // If we should not log...
            if (!IsEnabled(logLevel))
            {
                return;
            }

            object fileLock;

            // Double safety even though the file locks should be thread safe
            lock (FileLockLock)
            {
                // Get the file lock based on the absolute path
                fileLock = FileLocks.GetOrAdd(_filePath.ToUpper(), path => new object());
            }

            // Lock the file
            lock (fileLock)
            {
                // Ensure folder exists
                if (!Directory.Exists(_directory))
                {
                    Directory.CreateDirectory(_directory);
                }

                // Open the file
                using StreamWriter fileStream =
                    new StreamWriter(File.Open(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                                                         FileShare.ReadWrite));
                fileStream.BaseStream.Seek(0, SeekOrigin.End);

                // Write the message to the file
                fileStream.Write($"{logLevel}: {DateTimeOffset.Now:g} {formatter(state, exception)} \n");
            }
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