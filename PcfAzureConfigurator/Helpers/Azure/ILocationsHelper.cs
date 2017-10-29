using PcfAzureConfigurator.Helpers.Azure.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public interface ILocationsHelper
    {
        Task<Location[]> List(string environmentName, string token, string subscriptionId);
        Task<VmSize[]> ListVmSizes(string environmentName, string token, string subscriptionId, string location);
    }
}
