namespace Cored.Logging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The configuration for a <see cref="ILogger"/> implementations
    /// </summary>
    public class LoggerConfiguration
    {
        #region public Properties

        /// <summary>
        /// Gets or sets the level of the log that should be processed.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// Gets or sets a value indicating whether to log time as part of the message
        /// </summary>
        public bool LogTime { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to display latest logs at the top
        /// of the file.
        /// </summary>
        public bool LogAtTop { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the log level should be
        /// part of the log message.
        /// </summary>
        public bool OutputLogLevel { get; set; } = true;

        #endregion
    }
}