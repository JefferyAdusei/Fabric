using System;
using System.Xml.Serialization;

using Microsoft.Extensions.Logging;

namespace Cored.Logging.Xml
{
    /// <summary>
    /// The log serialization model
    /// </summary>
    [XmlRoot("Log")]
    public class Log
    {
        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        [XmlAttribute(nameof(LogLevel))]
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// The date the log is made.
        /// </summary>
        [XmlAttribute(nameof(Date), typeof(DateTime))]
        public string Date => DateTime.Now.ToString("g");

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        [XmlAttribute(nameof(FilePath), typeof(string))]
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        [XmlElement(nameof(Exception), typeof(string))]
        public string Exception { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [XmlElement(nameof(Message))]
        public Message Message { get; set; }
    }

    /// <summary>
    /// The message model for the log
    /// </summary>
    [XmlRoot]
    public class Message
    {
        /// <summary>
        /// Gets or sets the file from which this message is coming from origin.
        /// </summary>
        [XmlAttribute(nameof(Origin), typeof(string))]
        public string Origin { get; set; }

        /// <summary>
        /// Gets or sets the line number the text was generated from.
        /// </summary>
        [XmlAttribute(nameof(Line), typeof(int))]
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the text of the message.
        /// </summary>
        [XmlText(typeof(string))]
        public string Text { get; set; }
    }
}