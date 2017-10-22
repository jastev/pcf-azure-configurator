using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers;
using PcfAzureConfigurator.Helpers.Azure;
using Microsoft.Extensions.Primitives;

namespace PcfAzureConfigurator.Controllers.Azure
{
    [Produces("application/json")]
    [Route("api/v0/azure/resourcegroups")]
    public class ResourceGroupsController : Controller
    {
        private IResourceGroupsHelper _resourceGroupsHelper;

        public ResourceGroupsController(IResourceGroupsHelper resourceGroupsHelper)
        {
            _resourceGroupsHelper = resourceGroupsHelper;
        }

        public async Task<JsonResult> Get(string environment, string subscriptionId, string resourceGroupName)
        {
            StringValues authorizationValues;
            HttpContext.Request.Headers.TryGetValue("Authorization", out authorizationValues);
            if (authorizationValues.Count != 1) { /* TODO */ }
            var token = authorizationValues.First().Split(' ', 2)[1];  // Split off the preceding token type (should be "Bearer")

            JsonResult result;
            try
            {
                var resourceGroup = await _resourceGroupsHelper.Get(environment, token, subscriptionId, resourceGroupName);
                result = new JsonResult(new { result = resourceGroup });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { error = e.Response.Content });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }
    }
}