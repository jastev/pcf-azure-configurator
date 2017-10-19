using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.OpsManager
{
    public class UaaHelper : IUaaHelper
    {
        private IHttpClientProvider _httpclientProvider;

        public UaaHelper(IHttpClientProvider httpClientProvider)
        {
            _httpclientProvider = httpClientProvider;
        }

        public async Task<string> GetToken(string opsManagerFqdn, string username, string password)
        {
            var uri = "https://" + opsManagerFqdn + "/uaa/oauth/token";
            var parameters = new Dictionary<string, string>
            {
                { "client_id", "opsman" },
                { "client_secret", "" },
                { "grant_type", "password" },
                { "token_format", "opaque" },
                { "response_type", "token" },
                { "username", username },
                { "password", password }
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable<KeyValuePair<string, string>>());

            var response = await _httpclientProvider.HttpClient.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                return tokenResponse.access_token;
            }
            else
            {
                throw new HttpRequestException(response.Content.ToString());
            }
        }
    }

    class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string jti { get; set; }
    }
}
