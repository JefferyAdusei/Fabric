namespace Cored.Web
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using Mimes;

    /// <summary>
    /// Provides HTTP calls for sending and receiving information from a HTTP server
    /// </summary>
    public static class WebRequests
    {
        #region Get Request

        /// <summary>
        /// Send a GET request to an URL and returns the raw http web response
        /// </summary>
        /// <remarks>IMPORTANT: Remember to close the returned <see cref="HttpWebResponse"/> stream once done</remarks>
        /// <param name="url">The URL to make the request to</param>
        /// <param name="configureRequest">
        ///     Allows caller to customize and configure the request prior to the request being sent</param>
        /// <param name="bearerToken">
        ///     Provides the Authorization header with 'bearer token-here' for things like JWT bearer tokens
        /// </param>
        /// <returns>An <see cref="HttpWebResponse"/></returns>
        public static async Task<HttpWebResponse> GetAsync(string url, Action<HttpWebRequest> configureRequest = null,
            string bearerToken = null)
        {
            // Create the web request
            HttpWebRequest request = WebRequest.CreateHttp(url);

            // Make it a GET request method
            request.Method = WebRequestMethods.Http.Get;

            // If we have a bearer token...
            if (bearerToken != null)
            {
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {bearerToken}");
            }

            // Any custom work
            configureRequest?.Invoke(request);

            try
            {
                // Return the raw server response
                return await request.GetResponseAsync() as HttpWebResponse;
            }
            catch (WebException exception)
            {
                // If we got a response...
                if (exception.Response is HttpWebResponse httpResponse)
                {
                    // Return the response
                    return httpResponse;
                }

                // Otherwise, we don't have any information to be able to return
                throw;
            }
        }

        #endregion

        #region General Post Request

        /// <summary>
        /// Sends a POST request to an URL and returns the raw <see cref="HttpWebResponse"/>
        /// </summary>
        /// <remarks>IMPORTANT: Remember to close the returned <see cref="HttpWebResponse"/> once done.</remarks>
        /// <param name="url">The URL to make the request to</param>
        /// <param name="content">The content to post</param>
        /// <param name="sendMimeType">The format to serialize the content to</param>
        /// <param name="returnMimeType">The expected type of content to be returned from the server</param>
        /// <param name="configureRequest">Allows caller to customize and configure the request prior to the content being written and sent</param>
        /// <param name="bearerToken">Provides the Authorization header with 'Bearer {token}' for JWT bearer tokens</param>
        /// <returns><see cref="HttpWebResponse"/></returns>
        public static async Task<HttpWebResponse> PostAsync(string url, object content = null,
            MimeTypes sendMimeType = MimeTypes.Json,
            MimeTypes returnMimeType = MimeTypes.Json, Action<HttpWebRequest> configureRequest = null,
            string bearerToken = null)
        {
            // Create the web request
            HttpWebRequest request = WebRequest.CreateHttp(url);

            // Make it a POST request method
            request.Method = WebRequestMethods.Http.Post;

            // Set content type
            request.ContentType = sendMimeType.ToMimeText();

            // Set the appropriate return type
            request.Accept = returnMimeType.ToMimeText();

            // If we have a bearer token, then add it
            if (bearerToken != null)
            {
                // Add bearer token to header
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {bearerToken}");
            }

            // Configure custom request
            configureRequest?.Invoke(request);

            #region Write Content

            // Set the content length
            if (content == null)
            {
                // Set content length to 0
                request.ContentLength = 0;
            }
            else
            {
                string contentString;

                switch (sendMimeType)
                {
                    case MimeTypes.Json:
                        contentString = JsonConvert.SerializeObject(content);
                        break;

                    case MimeTypes.Xml:
                        {
                            // Create XML serializer
                            XmlSerializer xmlSerializer = new XmlSerializer(content.GetType());

                            // Create a string writer to receive the serialized string
                            using StringWriter stringWriter = new StringWriter();

                            // Serialize the object to a string
                            xmlSerializer.Serialize(stringWriter, content);

                            // Extract the string from the writer
                            contentString = stringWriter.ToString();
                            break;
                        }
                    default:
                        return null;
                }

                // Get body stream...
                using Stream requestStream = await request.GetRequestStreamAsync();

                /*
                 * NOTE: GetRequestStreamAsync could throw with a SocketException (or an inner exception
                 *       of SocketException)
                 *
                 *       However, we cannot return anything useful from this so we just let it throw out
                 *       so the caller can handle this (the other PostAsync call for example).
                 *
                 *       SocketExceptions are a good indication there is no Internet, or no connection or
                 *       firewalls blocking communication.
                 */

                // Create a stream writer from the body stream..
                using StreamWriter streamWriter = new StreamWriter(requestStream);

                // Write content to HTTP body stream
                await streamWriter.WriteAsync(contentString);
            }

            #endregion

            try
            {
                // Return the raw server response
                return await request.GetResponseAsync() as HttpWebResponse;
            }
            catch (WebException exception)
            {
                // If we got a response...
                if (exception.Response is HttpWebResponse httpWebResponse)
                {
                    // Return the response
                    return httpWebResponse;
                }

                // If there is no information, throw an exception
                throw;
            }
        }

        #endregion

        #region Post Request with Type

        /// <summary>
        /// Sends a POST request to an URL and returns a response of the expected data type <see cref="TResponse"/>
        /// </summary>
        /// <typeparam name="TResponse">Expected type of the web response</typeparam>
        /// <param name="url">The URL to make the request to</param>
        /// <param name="content">The content to post</param>
        /// <param name="sendMimeType">The format to serialize the content to</param>
        /// <param name="returnMimeType">The expected type of content to be returned from the server</param>
        /// <param name="configureRequest">Allows caller to customize and configure the request prior to the content being written and sent</param>
        /// <param name="bearerToken">Provides the Authorization header with 'Bearer {token}' for JWT bearer tokens</param>
        /// <returns>Web response of Type <see cref="TResponse"/>/></returns>
        public static async Task<WebResponse<TResponse>> PostAsync<TResponse>(string url, object content = null,
            MimeTypes sendMimeType = MimeTypes.Json,
            MimeTypes returnMimeType = MimeTypes.Json, Action<HttpWebRequest> configureRequest = null,
            string bearerToken = null)
        {
            // Create server response holder
            HttpWebResponse serverResponse;

            try
            {
                // Make the standard POST call first
                serverResponse = await PostAsync(url, content, sendMimeType, returnMimeType, configureRequest,
                                                 bearerToken);
            }
            catch (Exception exception)
            {
                // If we got unexpected error, return that
                return new WebResponse<TResponse>()
                {
                    // Include exception message
                    ErrorMessage = exception.Message
                };
            }

            // Create a result
            WebResponse<TResponse> result = await serverResponse.CreateWebResponseAsync<TResponse>();

            // If the response status code is not 200...
            if (result.StatusCode != HttpStatusCode.OK)
            {
                /*
                 * Call failed
                 * Return no error message so the client can display its own based on the status code
                 */
                return result;
            }

            // If we have no content to deserialize...
            if (string.IsNullOrEmpty(result.RawServerResponse))
            {
                return result;
            }

            // Deserialize a raw response
            try
            {
                // If the server response type was not the expected type...
                if (!serverResponse.ContentType.ToLower().Contains(returnMimeType.ToMimeText().ToLower()))
                {
                    // Fail due to unexpected return type
                    result.ErrorMessage =
                        $"Server did not return data in expected type. Expected {returnMimeType.ToMimeText()}, but received {serverResponse.ContentType}";
                    return result;
                }

                switch (returnMimeType)
                {
                    // JSON
                    case MimeTypes.Json:
                        result.ServerResponse = JsonConvert.DeserializeObject<TResponse>(result.RawServerResponse);
                        break;

                    // XML
                    case MimeTypes.Xml:
                        {
                            // Create XML serializer
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TResponse));

                            // Create a memory stream for the raw string data
                            await using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(result.RawServerResponse));

                            // Deserialize XML string
                            result.ServerResponse = (TResponse)xmlSerializer.Deserialize(memoryStream);

                            break;
                        }

                    // YAML
                    case MimeTypes.Yaml:
                        break;

                    // TEXT
                    case MimeTypes.Text:
                        break;

                    // UNKNOWN
                    default:
                        {
                            // If deserialize failed, then set error message
                            result.ErrorMessage =
                                "Unknown return type, cannot deserialize server response to the expected type";

                            return result;
                        }
                }
            }
            catch (Exception exception)
            {
                // If deserialize failed then set error message
                result.ErrorMessage = exception.Message;

                return result;
            }

            // Return result
            return result;
        }

        #endregion
    }
}