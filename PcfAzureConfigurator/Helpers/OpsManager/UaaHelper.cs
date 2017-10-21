using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.OpsManager
{
    public class UaaHelper : IUaaHelper
    {
        private IOauthHelper _oauthHelper;

        public UaaHelper(IOauthHelper oauthHelper)
        {
            _oauthHelper = oauthHelper;
        }

        public async Task<OauthToken> GetToken(string opsManagerFqdn, string username, string password)
        {
            var uri = "https://" + opsManagerFqdn + "/uaa/oauth/token";
            var parameters = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "opsman" },
                { "client_secret", "" },
                { "token_format", "opaque" },
                { "response_type", "token" },
                { "username", username },
                { "password", password }
            };

            return await _oauthHelper.GetToken(uri, parameters);
        }
    }
}
