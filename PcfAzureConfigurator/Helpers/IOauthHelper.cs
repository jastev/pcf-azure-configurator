using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers
{
    public interface IOauthHelper
    {
        Task<OauthToken> GetToken(string uri, Dictionary<string, string> parameters);
    }
}
