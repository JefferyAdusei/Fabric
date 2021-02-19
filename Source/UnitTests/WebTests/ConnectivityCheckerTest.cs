namespace UnitTests.WebTests
{
    using Cored.Web;
    using Xunit;

    public class ConnectivityCheckerTest
    {
        [Theory]
        [InlineData("https://google.com")]
        [InlineData("https://thioneshouldntworkatall.com")]
        public async void Ping_Okay(string url)
        {
            bool ok;

            using ConnectivityChecker checker = new ConnectivityChecker();

            ok = await checker.PingAsync(url);

            Assert.Equal(true, ok);
        }
    }
}