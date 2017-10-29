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
            { "AzureCloud", new AzureEnvironment(
                endpoints: new Endpoints(
                    activeDirectory: "https://login.microsoftonline.com",
                    resourceManager: "https://management.azure.com"
                    ),
                armApiVersions: new ArmApiVersions(
                    resourceGroups: "2017-05-10",
                    storage: "2017-06-01",
                    subscriptions: "2016-06-01",
                    virtualMachines: "2016-04-30-preview"
                    )
                ) },
            /*
            { "AzureUSGovernment", new AzureEnvironment(
                endpoints: new Endpoints(
                    activeDirectory: "https://login-us.microsoftonline.com", 
                    resourceManager: "https://management.usgovcloudapi.net"
                    ),
                armApiVersions: new ArmApiVersions(
                    resourceGroups: null
                    )
                ) },
            { "AzureGermanCloud", new AzureEnvironment(
                endpoints: new Endpoints(
                    activeDirectory: "https://login.microsoftonline.de",
                    resourceManager: "https://management.microsoftazure.de",
                    ),
                armApiVersions: new ArmApiVersions(
                    resourceGroups: null
                    )
                ) },
                */
            // Not currently suppoorted in OpsManager
            // { "AzureChinaCloud", new AzureEnvironment(activeDirectoryEndpointUrl: "https://login.chinacloudapi.cn", resourceManagerEndpointUrl: "https://management.chinacloudapi.cn") },
        };

    }

    public class AzureEnvironment
    {
        public Endpoints Endpoints { get; private set; }
        public ArmApiVersions ArmApiVersions { get; private set; }

        public AzureEnvironment(Endpoints endpoints, ArmApiVersions armApiVersions)
        {
            Endpoints = endpoints;
            ArmApiVersions = armApiVersions;
        }
    }

    public class Endpoints
    {
        public string ActiveDirectory { get; set; }
        public string ResourceManager { get; set; }
        public string Storage { get; set; }

        public Endpoints(string activeDirectory, string resourceManager)
        {
            ActiveDirectory = activeDirectory;
            ResourceManager = resourceManager;
        }
    }

    public class ArmApiVersions
    {
        public string ResourceGroups { get; set; }
        public string Storage { get; set; }
        public string Subscriptions { get; set; }
        public string VirtualMachines { get; set; }

        public ArmApiVersions(string resourceGroups, string storage, string subscriptions, string virtualMachines)
        {
            ResourceGroups = resourceGroups;
            Storage = storage;
            Subscriptions = subscriptions;
            VirtualMachines = virtualMachines;
        }
    }
}
