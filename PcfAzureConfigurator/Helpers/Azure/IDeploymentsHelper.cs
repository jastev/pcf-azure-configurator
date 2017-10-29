using PcfAzureConfigurator.Helpers.Azure.Deployments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public interface IDeploymentsHelper
    {
        Task<Deployment[]> List(string environmentName, string token, string subcriptionId, string resourceGroupName);
        Task Create(string environmentName, string token, string subcriptionId, string resourceGroupName, string deploymentName, DeploymentProperties properties);
        Task<Deployment> Get(string environmentName, string token, string subcriptionId, string resourceGroupName, string deploymentName);
    }
}
