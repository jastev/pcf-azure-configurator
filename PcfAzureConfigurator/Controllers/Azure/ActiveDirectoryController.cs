using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers;
using PcfAzureConfigurator.Helpers.Azure;

namespace PcfAzureConfigurator.Controllers
{
    [Route("api/v0/azure/{environment}/activedirectory/{tenantId}")]
    [Produces("application/json")]
    public class ActiveDirectoryController : Controller
    {
        private IActiveDirectoryHelper _activeDirectoryHelper;

        public ActiveDirectoryController(IActiveDirectoryHelper activeDirectoryHelper)
        {
            _activeDirectoryHelper = activeDirectoryHelper;
        }

        [Route("client/{clientId}/authenticate")]
        [HttpPost]
        public async Task<JsonResult> GetToken(string environment, string tenantId, string clientId, [FromBody] AuthenticationRequest authenticationRequest)
        {
            JsonResult result;
            try
            {
                var token = await _activeDirectoryHelper.GetToken(environment, tenantId, clientId, authenticationRequest.ClientSecret);
                result = new JsonResult(new { result = token });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { error = new { code = (int)e.Response.StatusCode, data = e.Response.Content } });
                result.StatusCode = (int) e.Response.StatusCode;
            }
            return result;
        }
    }

    public class AuthenticationRequest
    {
        public string ClientSecret { get; set; }
    }
}