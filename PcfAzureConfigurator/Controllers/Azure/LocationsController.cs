using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers.Azure;
using PcfAzureConfigurator.Helpers;
using Microsoft.Extensions.Primitives;

namespace PcfAzureConfigurator.Controllers.Azure
{
    [Route("api/v0/azure/{environment}/subscriptions/{subscriptionId}/locations")]
    [Produces("application/json")]
    public class LocationsController : Controller
    {
        private ILocationsHelper _locationsHelper;

        public LocationsController(ILocationsHelper locationsHelper)
        {
            _locationsHelper = locationsHelper;
        }

        [HttpGet]
        public async Task<JsonResult> List(string environment, string subscriptionId)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var locations = await _locationsHelper.List(environment, token, subscriptionId);
                result = new JsonResult(new { result = locations });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = e.Response.Content } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{location}/vmsizes")]
        [HttpGet]
        public async Task<JsonResult> ListVmSizes(string environment, string subscriptionId, string location)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var vmSizes = await _locationsHelper.ListVmSizes(environment, token, subscriptionId, location);
                result = new JsonResult(new { result = vmSizes });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = e.Response.Content } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }
    }
}