using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers;
using PcfAzureConfigurator.Helpers.OpsManager;

namespace PcfAzureConfigurator.Controllers
{
    [Produces("application/json")]
    [Route("api/v0/opsmanager/uaa")]
    public class UaaController : Controller
    {
        private IUaaHelper _uaaHelper;

        public UaaController(IUaaHelper uaaHelper)
        {
            _uaaHelper = uaaHelper;
        }

        [Route("token")]
        [HttpPost]
        public async Task<JsonResult> PostJson([FromBody] UaaTokenRequest request)
        {
            JsonResult result;
            try
            {
                var token = await _uaaHelper.GetToken(request.OpsManagerFqdn, request.Username, request.Password);
                result = new JsonResult(new { result = token });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { error = e.Response.Content });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        public class UaaTokenRequest
        {
            public string OpsManagerFqdn { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
