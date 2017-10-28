using PcfAzureConfigurator.Helpers.Azure.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public interface ISubscriptionsHelper
    {
        Task<Subscription[]> List(string environmentName, string token);
    }
}
