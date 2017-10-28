using PcfAzureConfigurator.Helpers.Azure.Deployments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public class DeploymentsHelper : IDeploymentsHelper
    {
        public Task<Deployment[]> List(string environmentName, string token, string subcriptionId, string resourceGroupName)
        {
            return null;
        }
    }
}

namespace PcfAzureConfigurator.Helpers.Azure.Deployments
{
    public class Deployment
    {

    }
}