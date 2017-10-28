using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers
{
    // The sole purpose of this class is to enable the definition of the
    // interface IHttpClient, which does not otherwise exist, so as to allow
    // a mock IHttpClient, conforming to that interface, to be supplied
    // during testing of the various Helpers

    public class HttpClientAdapter : IHttpClient
    {
        private HttpClient _httpClient;

        public HttpClientAdapter()
        {
            // Disable SSL validation
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            _httpClient = new HttpClient(handler);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message)
        {
            return _httpClient.SendAsync(message);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
