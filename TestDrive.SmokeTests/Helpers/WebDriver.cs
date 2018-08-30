using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestDrive.SmokeTests.Helpers
{
    public class WebDriver
    {
        private HttpClient _httpClient;
        public HttpResponseMessage _httpResponseMessage;

        public void InitializeHttpClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task HttpClientGet(string url)
        {
            _httpResponseMessage = await _httpClient.GetAsync(url);
        }

        public void CheckResponseStatusCode(int expectedStatusCode)
        {
            Assert.AreEqual(expectedStatusCode, _httpResponseMessage.StatusCode);
        }
    }
}
