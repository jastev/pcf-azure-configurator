using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public class ResourceGroupsHelper : IResourceGroupsHelper
    {
        private IHttpClientProvider _httpClientProvider;

        public ResourceGroupsHelper(IHttpClientProvider httpClientProvider)
        {
            _httpClientProvider = httpClientProvider;
        }

        public async Task<ResourceGroup> Get(string environmentName, string token, string subscriptionId, string resourceGroupName)
        {
            AzureEnvironment environment;
            EnvironmentHelper.Environments.TryGetValue(environmentName, out environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourcegroups/" + resourceGroupName + "?api-version=" + environment.ArmApiVersions.ResourceGroups );
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClientProvider.HttpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var resourceGroup = JsonConvert.DeserializeObject<ResourceGroup>(responseContent);

                return resourceGroup;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }
    }

    public class ResourceGroup
    {
        public string Id { get; set; }
        public string Location { get; set; }
        public string ManagedBy { get; set; }
        public string Name { get; set; }
        public ResourceGroupProperties Properties { get; set; }
    }

    public class ResourceGroupProperties
    {
        public string ProvisioningState { get; set; }
    }
}
