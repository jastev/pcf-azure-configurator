using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public interface IActiveDirectoryHelper
    {
        Task<OauthToken> GetToken(string environment, string tenantId, string clientId, string clientSecret);
    }
}
