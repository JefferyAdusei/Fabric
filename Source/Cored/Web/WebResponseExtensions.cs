namespace Cored.Web
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="WebResponse"/>
    /// </summary>
    public static class WebResponseExtensions
    {
        #region Create Web Response

        /// <summary>
        /// Returns a <see cref="WebResponse{T}"/> pre-populated with the <see cref="HttpWebResponse"/> information
        /// </summary>
        /// <typeparam name="TResponse">The type of response to create</typeparam>
        /// <param name="serverResponse">The response sent from the server</param>
        /// <returns>Web response of type {TResponse}</returns>
        public static WebResponse<TResponse> CreateWebResponse<TResponse>(this HttpWebResponse serverResponse)
        {
            // Return a new web request result
            WebResponse<TResponse> result = new WebResponse<TResponse>()
            {
                // Content type
                ContentType = serverResponse.ContentType,

                // Response uri
                ResponseUri = serverResponse.ResponseUri,

                // Status Description
                StatusDescription = serverResponse.StatusDescription,

                // Cookie collection
                Cookies = serverResponse.Cookies,

                // Headers
                Headers = serverResponse.Headers,

                // Status Code
                StatusCode = serverResponse.StatusCode
            };

            // If response is not successful
            if (result.StatusCode != HttpStatusCode.OK)
            {
                /*
                * Read in the response body
                * NOTE: By reading to the end of the stream, the stream will also close
                *       for us (which we must do to release the request)
                */
                return result;
            }

            // Open the response stream...
            using Stream responseStream = serverResponse.GetResponseStream();

            // Get a read stream
            using StreamReader streamReader = new StreamReader(responseStream!);

            /*
             * Read in the response body
             * NOTE: By reading to the end of the stream, the stream will also close
             *       for us (which we must do to release the request)
             */
            result.RawServerResponse = streamReader.ReadToEnd();

            return result;
        }

        #endregion

        #region Create Web Response Async

        /// <summary>
        /// Returns asynchronously a <see cref="WebResponse{T}"/> pre-populated with the <see cref="HttpWebResponse"/> information
        /// </summary>
        /// <typeparam name="TResponse">The type of response to create</typeparam>
        /// <param name="serverResponse">The response sent from the server</param>
        /// <returns>Web response of type {TResponse}</returns>
        public static async Task<WebResponse<TResponse>> CreateWebResponseAsync<TResponse>(this HttpWebResponse serverResponse)
        {
            // Return a new web request result
            WebResponse<TResponse> result = new WebResponse<TResponse>()
            {
                // Content type
                ContentType = serverResponse.ContentType,

                // Response uri
                ResponseUri = serverResponse.ResponseUri,

                // Status Description
                StatusDescription = serverResponse.StatusDescription,

                // Cookie collection
                Cookies = serverResponse.Cookies,

                // Headers
                Headers = serverResponse.Headers,

                // Status Code
                StatusCode = serverResponse.StatusCode
            };

            // If response is not successful
            if (result.StatusCode != HttpStatusCode.OK)
            {
                /*
                 * Return the server result literally because there is no raw server response to return.
                 * This is because the server status code returned a status code aside 200 thus the
                 * expected server response was not returned by the server.
                 */
                return result;
            }

            // Open the response stream...
            await using Stream responseStream = serverResponse.GetResponseStream();

            // Get a read stream
            using StreamReader streamReader = new StreamReader(responseStream!);

            /*
             * Read in the response body
             * NOTE: By reading to the end of the stream, the stream will also close
             *       for us (which we must do to release the request)
             */
            result.RawServerResponse = await streamReader.ReadToEndAsync();

            return result;
        }

        #endregion
    }
}