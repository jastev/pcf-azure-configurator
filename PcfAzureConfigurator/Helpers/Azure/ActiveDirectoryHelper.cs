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

        public async Task<OauthToken> GetToken(string environmentName, string tenantId, string clientId, string clientSecret)
        {
            AzureEnvironment environment;
            EnvironmentHelper.Environments.TryGetValue(environmentName, out environment);
            var uri = environment.Endpoints.ActiveDirectory + "/" + tenantId + "/oauth2/token";
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "resource", environment.Endpoints.ResourceManager + "/"}, // The slash is required for subsequent authorization of requests to the endpoint
                { "scope", "user_impersonation" },
            };

            return await _oauthHelper.GetToken(uri, parameters);
        }
    }
}
