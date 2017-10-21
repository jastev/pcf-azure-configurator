using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public static class EnvironmentHelper
    {
        public static readonly Dictionary<string, AzureEnvironment> Environments = new Dictionary<string, Azure.AzureEnvironment>
        {
            // These names correspond to the legal values for 'environment' in the OpsManager Director IaasConfiguration
            { "AzureCloud", new AzureEnvironment(activeDirectoryEndpointUrl: "https://login.microsoftonline.com", resourceManagerEndpointUrl: "https://management.azure.com") },
            { "AzureUSGovernment", new AzureEnvironment(activeDirectoryEndpointUrl: "https://login-us.microsoftonline.com", resourceManagerEndpointUrl: "https://management.usgovcloudapi.net") },
            { "AzureGermanCloud", new AzureEnvironment(activeDirectoryEndpointUrl: "https://login.microsoftonline.de", resourceManagerEndpointUrl: "https://management.microsoftazure.de") },
            // Not currently suppoorted in OpsManager
            // { "AzureChinaCloud", new AzureEnvironment(activeDirectoryEndpointUrl: "https://login.chinacloudapi.cn", resourceManagerEndpointUrl: "https://management.chinacloudapi.cn") },
        };

    }

    public class AzureEnvironment
    {
        public AzureEnvironment(string activeDirectoryEndpointUrl, string resourceManagerEndpointUrl)
        {
            ActiveDirectoryEndpointUrl = activeDirectoryEndpointUrl;
            ResourceManagerEndpointUrl = resourceManagerEndpointUrl;
        }

        public string ActiveDirectoryEndpointUrl { get; private set; }
        public string ResourceManagerEndpointUrl { get; private set; }
    }
}
