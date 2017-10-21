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
    [Produces("application/json")]
    [Route("api/v0/azure/activedirectory")]
    public class ActiveDirectoryController : Controller
    {
        private IActiveDirectoryHelper _activeDirectoryHelper;

        public ActiveDirectoryController(IActiveDirectoryHelper activeDirectoryHelper)
        {
            _activeDirectoryHelper = activeDirectoryHelper;
        }

        [Route("token")]
        [HttpPost]
        public async Task<JsonResult> PostJson([FromBody] ActiveDirectoryTokenRequest request)
        {
            JsonResult result;
            try
            {
                var token = await _activeDirectoryHelper.GetToken(request.Environment, request.TenantId, request.ClientId, request.ClientSecret);
                result = new JsonResult(new { token = token });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(e.Response.Content);
                result.StatusCode = (int) e.Response.StatusCode;
            }
            return result;
        }

        public class ActiveDirectoryTokenRequest
        {
            public string Environment { get; set; }
            public string TenantId { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}