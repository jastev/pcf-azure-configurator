using PcfAzureConfigurator.Helpers.Azure.ResourceGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public interface IResourceGroupsHelper
    {
        Task<ResourceGroup[]> List(string environmentName, string token, string subcriptionId);
        Task Create(string environmentName, string token, string subcriptionId, string resourceGroupName, ResourceGroup resourceGroup);
        Task<ResourceGroup> Get(string environmentName, string token, string subcriptionId, string resourceGroupName);
    }
}
