using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers;
using PcfAzureConfigurator.Helpers.Azure;
using Microsoft.Extensions.Primitives;
using PcfAzureConfigurator.Helpers.Azure.ResourceGroups;

namespace PcfAzureConfigurator.Controllers.Azure
{
    [Route("api/v0/azure/{environment}/subscriptions/{subscriptionId}/resourcegroups")]
    [Produces("application/json")]
    public class ResourceGroupsController : Controller
    {
        private IResourceGroupsHelper _resourceGroupsHelper;

        public ResourceGroupsController(IResourceGroupsHelper resourceGroupsHelper)
        {
            _resourceGroupsHelper = resourceGroupsHelper;
        }

        [HttpGet]
        public async Task<JsonResult> List(string environment, string subscriptionId)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var resourceGroups = await _resourceGroupsHelper.List(environment, token, subscriptionId);
                result = new JsonResult(new { result = resourceGroups });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { error = new { code = (int)e.Response.StatusCode, data = e.Response.Content } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{resourceGroupName}")]
        [HttpPut]
        public async Task<JsonResult> Create(string environment, string subscriptionId, string resourceGroupName, [FromBody] ResourceGroup resourceGroup)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                await _resourceGroupsHelper.Create(environment, token, subscriptionId, resourceGroupName, resourceGroup);
                result = new JsonResult(null);
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { error = new { code = (int)e.Response.StatusCode, data = e.Response.Content } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{resourceGroupName}")]
        [HttpGet]
        public async Task<JsonResult> Get(string environment, string subscriptionId, string resourceGroupName)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var resourceGroup = await _resourceGroupsHelper.Get(environment, token, subscriptionId, resourceGroupName);
                result = new JsonResult(new { result = resourceGroup });
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