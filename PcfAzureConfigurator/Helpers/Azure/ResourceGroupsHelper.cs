using Newtonsoft.Json;
using PcfAzureConfigurator.Helpers.Azure.ResourceGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public class ResourceGroupsHelper : IResourceGroupsHelper
    {
        private IHttpClient _httpClient;

        public ResourceGroupsHelper(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResourceGroup[]> List(string environmentName, string token, string subscriptionId)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourcegroups?api-version=" + environment.ArmApiVersions.ResourceGroups);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var resourceGroups = JsonConvert.DeserializeObject<ResourceGroupList>(responseContent).Value;

                return resourceGroups;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }

        public async Task Create(string environmentName, string token, string subscriptionId, string resourceGroupName, ResourceGroup resourceGroup)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourcegroups/" + resourceGroupName + "?api-version=" + environment.ArmApiVersions.ResourceGroups);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(resourceGroup), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpResponseException(response);
            }
        }

        public async Task<ResourceGroup> Get(string environmentName, string token, string subscriptionId, string resourceGroupName)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourcegroups/" + resourceGroupName + "?api-version=" + environment.ArmApiVersions.ResourceGroups);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

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
}

namespace PcfAzureConfigurator.Helpers.Azure.ResourceGroups
{
    public class ResourceGroupList
    {
        public ResourceGroup[] Value { get; set; }
    }

    // Only properties that are useful at read and required at create
    public class ResourceGroup
    {
        public string Location { get; set; }
        public string Name { get; set; }
        public ResourceGroupProperties Properties { get; set; }
    }

    public class ResourceGroupProperties
    {
        public string ProvisioningState { get; set; }
    }
}
