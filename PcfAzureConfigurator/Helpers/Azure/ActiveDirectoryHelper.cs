using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public class ActiveDirectoryHelper : IActiveDirectoryHelper
    {
        private IOauthHelper _oauthHelper;

        public ActiveDirectoryHelper(IOauthHelper oauthHelper)
        {
            _oauthHelper = oauthHelper;
        }

        public async Task<OauthToken> GetToken(string environment, string tenantId, string clientId, string clientSecret)
        {
            AzureEnvironment cloud;
            EnvironmentHelper.Environments.TryGetValue(environment, out cloud);
            var uri = cloud.ActiveDirectoryEndpointUrl + "/" + tenantId + "/oauth2/token";
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "resource", cloud.ResourceManagerEndpointUrl },
                { "scope", "user_impersonation" },
            };

            return await _oauthHelper.GetToken(uri, parameters);
        }
    }
}
