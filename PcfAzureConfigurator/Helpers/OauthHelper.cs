using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
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
        private IHttpClient _httpClient;

        public OauthHelper(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OauthToken> GetToken(string uri, Dictionary<string, string> parameters)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new FormUrlEncodedContent(parameters.AsEnumerable<KeyValuePair<string, string>>());
            var response = await _httpClient.SendAsync(request);

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

        public static string GetToken(IHeaderDictionary headers)
        {
            headers.TryGetValue("Authorization", out StringValues authorizationValues);
            if (authorizationValues.Count == 0)
            {
                return null; // There is no Authorization header
            }
            else if (authorizationValues.Count == 1)
            {
                var token = authorizationValues.First().Split(' ', 2)[1];  // Split off the preceding token type
                return token;
            }
            else
            {
                throw new Exception(); // TODO throw a more specific exception
            }
        }

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
