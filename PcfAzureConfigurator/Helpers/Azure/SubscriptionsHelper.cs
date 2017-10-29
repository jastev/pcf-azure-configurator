using Newtonsoft.Json;
using PcfAzureConfigurator.Helpers.Azure.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public class SubscriptionsHelper : ISubscriptionsHelper
    {
        private IHttpClient _httpClient;

        public SubscriptionsHelper(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Subscription[]> List(string environmentName, string token)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, environment.Endpoints.ResourceManager + "/subscriptions?api-version=" + environment.ArmApiVersions.Subscriptions);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var subscriptions = JsonConvert.DeserializeObject<SubscriptionList>(responseContent).Value;

                return subscriptions;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }
    }
}

namespace PcfAzureConfigurator.Helpers.Azure.Subscriptions
{
    public class SubscriptionList
    {
        public Subscription[] Value { get; set; }
    }

    // Since we're not creating Subscriptions, we can use an abbreviated set of properties
    public class Subscription
    {
        public string DisplayName { get; set; }
        public string SubscriptionId { get; set; }
    }
}
