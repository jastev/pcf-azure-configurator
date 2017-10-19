using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers.OpsManager;

namespace PcfAzureConfigurator.Controllers
{
    [Produces("application/json")]
    [Route("api/v0/token")]
    public class TokenController : Controller
    {
        private IUaaHelper _uaaHelper;

        public TokenController(IUaaHelper uaaHelper)
        {
            _uaaHelper = uaaHelper;
        }

        [HttpPost]
        public async Task<JsonResult> PostJson([FromBody] TokenRequest request)
        {
            string token = await _uaaHelper.GetToken(request.OpsManagerFqdn, request.Username, request.Password);
            return new JsonResult(new { token = token });
        }

        public class TokenRequest
        {
            public string OpsManagerFqdn { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
