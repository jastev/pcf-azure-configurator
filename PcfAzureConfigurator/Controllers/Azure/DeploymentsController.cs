using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers.Azure;
using PcfAzureConfigurator.Helpers;
using PcfAzureConfigurator.Helpers.Azure.Deployments;

namespace PcfAzureConfigurator.Controllers.Azure
{
    [Route("api/v0/azure/{environment}/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/deployments")]
    [Produces("application/json")]
    public class DeploymentsController : Controller
    {
        private IDeploymentsHelper _deploymentsHelper;

        public DeploymentsController(IDeploymentsHelper deploymentsHelper)
        {
            _deploymentsHelper = deploymentsHelper;
        }

        [HttpGet]
        public async Task<JsonResult> List(string environment, string subscriptionId, string resourceGroupName)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var deployments = await _deploymentsHelper.List(environment, token, subscriptionId, resourceGroupName);
                result = new JsonResult(new { result = deployments });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{deploymentName}")]
        [HttpPut]
        public async Task<JsonResult> Create(string environment, string subscriptionId, string resourceGroupName, string deploymentName, [FromBody] DeploymentProperties properties)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                await _deploymentsHelper.Create(environment, token, subscriptionId, resourceGroupName, deploymentName, properties);
                result = new JsonResult(new { result = true });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{deploymentName}")]
        [HttpGet]
        public async Task<JsonResult> Get(string environment, string subscriptionId, string resourceGroupName, string deploymentName)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var deployment = await _deploymentsHelper.Get(environment, token, subscriptionId, resourceGroupName, deploymentName);
                result = new JsonResult(new { result = deployment });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }
    }
}