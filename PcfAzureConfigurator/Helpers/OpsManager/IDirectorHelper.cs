using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.OpsManager
{
    public interface IDirectorHelper
    {
        Task<DirectorProperties> GetProperties(string opsManagerFqdn, string token);
    }
}
