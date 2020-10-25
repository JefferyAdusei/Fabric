namespace Cored.Web.Mimes
{
    /// <summary>
    /// Known mime types of content that can be serialized and sent to receiver.
    /// </summary>
    public enum MimeTypes
    {
        /// <summary>
        /// Data is serialized to JSON
        /// </summary>
        Json,

        /// <summary>
        /// Data is serialized to XML
        /// </summary>
        Xml,

        /// <summary>
        /// Data is serialized to Yaml
        /// </summary>
        Yaml,

        /// <summary>
        /// Data is serialized as plain text
        /// </summary>
        Text
    }
}