using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers
{
    public class HttpClientProvider : IHttpClientProvider, IDisposable
    {
        public HttpClient HttpClient { get; private set; }

        public HttpClientProvider()
        {
            // Disable SSL validation
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            HttpClient = new HttpClient(handler);
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}
