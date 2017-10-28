using Newtonsoft.Json;
using PcfAzureConfigurator.Helpers.Azure.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public class LocationsHelper : ILocationsHelper
    {
        private IHttpClient _httpClient;

        public LocationsHelper(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Location[]> List(string environmentName, string token, string subscriptionId)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/locations?api-version=" + environment.ArmApiVersions.Subscriptions);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var locations = JsonConvert.DeserializeObject<LocationList>(responseContent).Value;

                return locations;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }

        public async Task<VmSize[]> ListVmSizes(string environmentName, string token, string subscriptionId, string location)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/providers/Microsoft.Compute/locations/" + location + "/vmSizes?api-version=" + environment.ArmApiVersions.VirtualMachines);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var vmSizes = JsonConvert.DeserializeObject<VmSizeList>(responseContent).Value;

                return vmSizes;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }
    }
}

namespace PcfAzureConfigurator.Helpers.Azure.Locations
{
    public class LocationList
    {
        public Location[] Value { get; set; }
    }
    public class Location
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
    }

    public class VmSizeList
    {
        public VmSize[] Value { get; set; }
    }

    public class VmSize
    {
        public string Name { get; set; }
        public int NumberOfCores { get; set; }
        public int OsDiskSizeInMB { get; set; }
        public int ResourceDiskSizeInMB { get; set; }
        public int MemoryInMB { get; set; }
        public int MaxDataDiskCount { get; set; }
    }
}
