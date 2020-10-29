namespace Cored.Web
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Continually hits  up a web HTTP/HTTPS endpoint at a specified interval, checking for a valid response
    /// This is particularly helpful for checking the connectivity of a website.
    /// </summary>
    public class ConnectivityChecker
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

        public async Task<bool> PingAsync(string endpoint, Func<HttpWebResponse, bool> validResponseParser)
        {
            HttpWebResponse webResponse = await WebRequests.GetAsync(endpoint);

            bool responsive = validResponseParser?.Invoke(webResponse) ?? webResponse != null;

            webResponse?.Close();

            return responsive;
        }

        #endregion
    }
}