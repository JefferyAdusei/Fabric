namespace Cored.Logging
{
    using Microsoft.Extensions.Logging;

    #region Roll
    /// <summary>
    /// Enumerator for configuring roll over times for logs
    /// </summary>
    public enum Roll
    {
        /// <summary>
        /// Roll over log file yearly
        /// </summary>
        Yearly,

        /// <summary>
        /// Roll over log file monthly
        /// </summary>
        Monthly,

        /// <summary>
        /// Roll over log file weekly
        /// </summary>
        Weekly,

        /// <summary>
        /// Roll over log file daily
        /// </summary>
        Daily,
    }
    #endregion

    #region Configurator

    /// <summary>
    /// The configuration for a <see cref="ILogger"/> implementations
    /// </summary>
    public class Configurator
    {
        #region public Properties

        /// <summary>
        /// Gets or sets the path to log the file
        /// </summary>
        public string Path { get; set; } = "log";


        /// <summary>
        /// Gets or sets the level of the log that should be processed.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// Gets or sets the time event on which to roll the log to another file
        /// </summary>
        public Roll Roll { get; set; } = Roll.Daily;

        /// <summary>
        /// Gets or sets a value indicating whether the log level should be
        /// part of the log message.
        /// </summary>
        public string FilePath => Path.NormalizePath().ResolvePath();

        #endregion
    }
    #endregion
}