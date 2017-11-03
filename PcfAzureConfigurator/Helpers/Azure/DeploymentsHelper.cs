using Newtonsoft.Json;
using PcfAzureConfigurator.Helpers.Azure.Deployments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public class DeploymentsHelper : IDeploymentsHelper
    {
        private IHttpClient _httpClient;

        public DeploymentsHelper(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<Deployment[]> List(string environmentName, string token, string subscriptionId, string resourceGroupName)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourcegroups/" + resourceGroupName + "/providers/Microsoft.Resources/deployments?api-version=" + environment.ArmApiVersions.ResourceGroups);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var deployments = JsonConvert.DeserializeObject<DeploymentList>(responseContent).Results;

                return deployments;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }

        public async Task Create(string environmentName, string token, string subscriptionId, string resourceGroupName, string deploymentName, DeploymentProperties properties)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourcegroups/" + resourceGroupName + "/providers/Microsoft.Resources/deployments/" + deploymentName + "?api-version=" + environment.ArmApiVersions.ResourceGroups);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var deployment = new Deployment();
            deployment.Properties = properties;
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(deployment), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpResponseException(response);
            }
        }

        public async Task<Deployment> Get(string environmentName, string token, string subscriptionId, string resourceGroupName, string deploymentName)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourcegroups/" + resourceGroupName + "/providers/Microsoft.Resources/deployments/" + deploymentName + "?api-version=" + environment.ArmApiVersions.ResourceGroups);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var deployment = JsonConvert.DeserializeObject<Deployment>(responseContent);

                return deployment;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }
    }
}

namespace PcfAzureConfigurator.Helpers.Azure.Deployments
{
    public class DeploymentList
    {
        public Deployment[] Results { get; set; }
    }

    public class Deployment
    {
        public string Name { get; set; }
        public DeploymentProperties Properties { get; set; }
    }

    public class DeploymentProperties
    {
        public string Mode { get; set; }
        public Object Parameters { get; set; }
        public TemplateLink TemplateLink { get; set; }
    }

    public class TemplateLink
    {
        public string Uri { get; set; }
    }
}