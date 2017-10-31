using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers.Azure;
using Microsoft.Extensions.Primitives;
using PcfAzureConfigurator.Helpers;

namespace PcfAzureConfigurator.Controllers.Azure
{
    [Route("api/v0/azure/{environment}/subscriptions")]
    [Produces("application/json")]
    public class SubscriptionsController : Controller
    {
        private ISubscriptionsHelper _subscriptionsHelper;

        public SubscriptionsController(ISubscriptionsHelper subscriptionsHelper)
        {
            _subscriptionsHelper = subscriptionsHelper;
        }

        [HttpGet]
        public async Task<JsonResult> List(string environment)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var subscriptions = await _subscriptionsHelper.List(environment, token);
                result = new JsonResult(new { result = subscriptions });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { error = new { code = (int)e.Response.StatusCode, data = e.Response.Content } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }
    }
}