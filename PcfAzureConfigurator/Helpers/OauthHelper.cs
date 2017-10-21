using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers
{
    public class OauthHelper : IOauthHelper
    {
        private IHttpClientProvider _httpclientProvider;

        public OauthHelper(IHttpClientProvider httpClientProvider)
        {
            _httpclientProvider = httpClientProvider;
        }

        public async Task<OauthToken> GetToken(string uri, Dictionary<string, string> parameters)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable<KeyValuePair<string, string>>());
            var response = await _httpclientProvider.HttpClient.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<OauthToken>(responseContent);

                return token;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }
    }

    public class OauthToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }

        public OauthToken() { }
        public OauthToken(string token) { access_token = token; }

        new public string ToString() { return access_token; }

        public static AuthenticationHeaderValue GetAuthenticationHeader(string token)
        {
            return new AuthenticationHeaderValue("Bearer", token);
        }

        public AuthenticationHeaderValue GetAuthenticationHeader()
        {
            return GetAuthenticationHeader(access_token);
        }
    }
}
