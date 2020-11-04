using System.Xml.Schema;

namespace Cored.Web
{
    using System;
    using System.Net;
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

        /// <summary>
        /// Gets or sets an action callback when the connectivity state changes
        /// </summary>
        private Action<bool> _stateChangedCallback;

        /// <summary>
        /// Gets or sets a value indicating whether there has been a call to the endpoint yet,
        /// or this is the first call.
        /// </summary>
        private bool _firstCallMade;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the endpoint being checked
        /// </summary>
        public string Endpoint { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the endpoint is responsive
        /// </summary>
        public bool Responsive { get; set; }

        #endregion

        #region Connectivity Methods

        public async Task<bool> PingAsync(string endpoint, Func<HttpWebResponse, bool> validResponseParser = null)
        {
            HttpWebResponse webResponse = await WebRequests.GetAsync(endpoint);

            bool responsive = validResponseParser?.Invoke(webResponse) ?? webResponse != null;

            webResponse?.Close();

            return responsive;
        }

        /// <summary>
        /// Hits up a web HTTP/HTTPS endpoint continually at a given interval to determine whether the server
        /// is responsive.
        /// </summary>
        /// <param name="endpoint">The endpoint to do a GET call on</param>
        /// <param name="interval">The time between periodical checks in milliseconds</param>
        /// <param name="stateChangedCallback">Fired when the endpoint state changes (responsive/not responsive)</param>
        /// <param name="validResponseParser">
        ///     If specified, handles whether the <see cref="HttpWebResponse"/> from the server should be classified
        ///     as successful or not.
        /// </param>
        /// <returns></returns>
        public async Task LinearRetryAsync(string endpoint, TimeSpan interval, Action<bool> stateChangedCallback,
            Func<HttpWebResponse, bool> validResponseParser = null)
        {
            while (!_disposing)
            {
                /*
                 * Assume that any response from the server is valid even it is a
                 * 401. This is because a page not found or server error actually
                 * is a server response.
                 */
                HttpWebResponse webResponse = await WebRequests.GetAsync(endpoint);

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
                // Close the web response
                webResponse?.Close();

                // Delay for the given timespan before starting over the loop.
                await Task.Delay(interval);
            }
        }

/// <summary>
        /// Hits up a web HTTP/HTTPS endpoint continually at a given interval to determine whether the server
        /// is responsive.
        /// </summary>
        /// <param name="endpoint">The endpoint to do a GET call on</param>
        /// <param name="interval">The time between periodical checks in milliseconds</param>
        /// <param name="stateChangedCallback">Fired when the endpoint state changes (responsive/not responsive)</param>
        /// <param name="validResponseParser">
        ///     If specified, handles whether the <see cref="HttpWebResponse"/> from the server should be classified
        ///     as successful or not.
        /// </param>
        /// <returns></returns>
        public async Task ExponentialRetryAsync(string endpoint, TimeSpan interval, Action<bool> stateChangedCallback,
            Func<HttpWebResponse, bool> validResponseParser = null)
        {
            int exponent = 0;

            while (!_disposing)
            {
                /*
                 * Assume that any response from the server is valid even it is a
                 * 401. This is because a page not found or server error actually
                 * is a server response.
                 */
                HttpWebResponse webResponse = await WebRequests.GetAsync(endpoint);

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
                // Close the web response
                webResponse?.Close();

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