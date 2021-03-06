﻿namespace Cored.Web
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Continually hits  up a web HTTP/HTTPS endpoint at a specified interval, checking for a valid response
    /// This is particularly helpful for checking the connectivity of a website.
    /// </summary>
    public class ConnectivityChecker : IDisposable
    {
        #region Private Members

        /// <summary>
        /// Gets or sets a flag indicating whether the class is disposing.
        /// </summary>
        private bool _disposing;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the endpoint is responsive
        /// </summary>
        private bool Responsive { get; set; }

        #endregion

        #region Connectivity Methods

        /// <summary>
        /// Hits up a web HTTP/HTTPS endpoint once to determine whether the endpoint is active, just like a ping does.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endpoint">The endpoint to do a get call on</param>
        /// <param name="validResponseParser">
        ///     If specified, handles whether the <see cref="HttpResponseMessage"/> from the server should be classified
        ///     as successful or not.
        /// </param>
        /// <returns>A value indicating whether the endpoint is active</returns>
        public async Task<bool> PingAsync(HttpClient client, string endpoint, Func<HttpResponseMessage, bool> validResponseParser = null)
        {
            using HttpResponseMessage webResponse = await client.GetAsync(endpoint);

            bool responsive = validResponseParser?.Invoke(webResponse) ?? webResponse != null;

            return responsive;
        }

        /// <summary>
        /// Hits up a web HTTP/HTTPS endpoint continually at a given interval to determine whether the server
        /// is responsive.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endpoint">The endpoint to do a GET call on</param>
        /// <param name="interval">The time between periodical checks in milliseconds</param>
        /// <param name="stateChangedCallback">Fired when the endpoint state changes (responsive/not responsive)</param>
        /// <param name="validResponseParser">
        ///     If specified, handles whether the <see cref="HttpResponseMessage"/> from the server should be classified
        ///     as successful or not.
        /// </param>
        /// <returns>A value indicating whether the endpoint is active</returns>
        public async Task LinearRetryAsync(HttpClient client, string endpoint, TimeSpan interval, Action<bool> stateChangedCallback,
            Func<HttpResponseMessage, bool> validResponseParser = null)
        {
            while (!_disposing)
            {
                /*
                 * Assume that any response from the server is valid even it is a
                 * 401. This is because a page not found or server error actually
                 * is a server response.
                 */
                using HttpResponseMessage webResponse = await client.GetAsync(endpoint);

                /*
                 * If there is a valid response parser, ask it for the state based on the response
                 * otherwise, so long as there is a response of any kind, it's valid.
                 */
                bool responsive = validResponseParser?.Invoke(webResponse) ?? webResponse != null;

                /*
                 * If the server is returns a value different from the value of the responsive
                 * property, then set the responsive property to the server response.
                 * This is to make sure that we only invoke the stateChangedCallback only when
                 * there is a change in the server response
                 */
                if (responsive != Responsive)
                {
                    Responsive = responsive;

                    // Inform the action about the change in the network/connectivity
                    stateChangedCallback?.Invoke(responsive);
                }

                // Delay for the given timespan before starting over the loop.
                await Task.Delay(interval);
            }
        }

        /// <summary>
        /// Hits up a web HTTP/HTTPS endpoint continually at a given interval to determine whether the server
        /// is responsive.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endpoint">The endpoint to do a GET call on</param>
        /// <param name="interval">The time between periodical checks in milliseconds</param>
        /// <param name="stateChangedCallback">Fired when the endpoint state changes (responsive/not responsive)</param>
        /// <param name="validResponseParser">
        ///     If specified, handles whether the <see cref="HttpResponseMessage"/> from the server should be classified
        ///     as successful or not.
        /// </param>
        /// <returns>A value indicating whether the endpoint is active</returns>
        public async Task ExponentialRetryAsync(HttpClient client, string endpoint, TimeSpan interval, Action<bool> stateChangedCallback,
            Func<HttpResponseMessage, bool> validResponseParser = null)
        {
            int exponent = 0;

            while (!_disposing)
            {
                /*
                 * Assume that any response from the server is valid even it is a
                 * 401. This is because a page not found or server error actually
                 * is a server response.
                 */
                using HttpResponseMessage webResponse = await client.GetAsync(endpoint);

                /*
                 * If there is a valid response parser, ask it for the state based on the response
                 * otherwise, so long as there is a response of any kind, it's valid.
                 */
                bool responsive = validResponseParser?.Invoke(webResponse) ?? webResponse != null;

                /*
                 * If the server is returns a value different from the value of the responsive
                 * property, then set the responsive property to the server response.
                 * This is to make sure that we only invoke the stateChangedCallback only when
                 * there is a change in the server response
                 */
                if (responsive != Responsive)
                {
                    Responsive = responsive;

                    // Inform the action about the change in the network/connectivity
                    stateChangedCallback?.Invoke(responsive);
                }

                /*
                 * For every new attempt, raise the interval by an exponential value.
                 * This is to ensure that the frequency of the web request does not overwhelm the server.
                 * Because for every new request the time to retry is slightly higher than the previous.
                 */
                interval = TimeSpan.FromMilliseconds(interval.Milliseconds * Math.Pow(2, exponent++));

                // Delay for the given timespan before starting over the loop.
                await Task.Delay(interval);
            }
        }
        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            _disposing = true;
        }

        #endregion
    }
}