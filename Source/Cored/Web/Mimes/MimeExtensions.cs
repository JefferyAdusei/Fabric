namespace Cored.Web.Mimes
{
    /// <summary>
    /// Extension methods for <see cref="MimeTypes"/>
    /// </summary>
    public static class MimeExtensions
    {
        /// <summary>
        /// Takes a known serializer type and return the mime type associated with it
        /// </summary>
        /// <param name="mimeType">The serializer</param>
        /// <returns>The mime text</returns>
        public static string ToMimeText(this MimeTypes mimeType)
        {
            return mimeType switch
            {
                // JSON
                MimeTypes.Json => "application/json",
                // XML
                MimeTypes.Xml => "application/xml",
                // YAML
                MimeTypes.Yaml => "application/yaml",
                // Text
                MimeTypes.Text => "application/text",
                // Default
                _ => "application/octet-stream"
            };
        }
    }
}