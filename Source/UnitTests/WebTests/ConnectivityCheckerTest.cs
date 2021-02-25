using System.Net.Http;

namespace UnitTests.WebTests
{
    using Cored.Web;
    using Xunit;

    public class ConnectivityCheckerTest
    {
        #region Properties

        /// <summary>
        /// Gets the HttpClient that would be used in making all requests.
        /// </summary>
        private static readonly HttpClient HttpClient = new HttpClient();

        #endregion

        [Theory]
        [InlineData("https://google.com")]
        [InlineData("https://thioneshouldntworkatall.com")]
        public async void Ping_Okay(string url)
        {
            using ConnectivityChecker checker = new ConnectivityChecker();

            var ok = await checker.PingAsync(HttpClient, url);

            Assert.True(ok);
        }
    }
}