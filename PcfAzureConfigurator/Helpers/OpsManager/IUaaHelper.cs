using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.OpsManager
{
    public interface IUaaHelper
    {
        Task<string> GetToken(string opsManagerFqdn, string username, string password);
    }
}
