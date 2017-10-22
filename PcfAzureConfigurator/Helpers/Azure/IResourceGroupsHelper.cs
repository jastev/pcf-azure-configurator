using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public interface IResourceGroupsHelper
    {
        Task<ResourceGroup> Get(string environmentName, string token, string subcriptionId, string resourceGroupName);
    }
}
