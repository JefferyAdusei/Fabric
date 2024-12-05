using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace Cored.Logging.Model
{

    [XmlRoot]
    internal class Log
    {
        [XmlElement]
        public LogLevel LogLevel { get; set; }

        [XmlAttribute]
        public string Date { get; set; }

        [XmlAttribute]
        public string FilePath { get; set; }

        [XmlElement]
        public string Exception { get; set; }

        [XmlElement]
        public Details Details { get; set; }

    }

    [XmlRoot]
    internal class Details
    {
        [XmlAttribute]
        public string Origin { get; set; }

        [XmlAttribute]
        public string LineNumber { get; set; }

        [XmlElement]
        public string Message { get; set; }
    }
}
