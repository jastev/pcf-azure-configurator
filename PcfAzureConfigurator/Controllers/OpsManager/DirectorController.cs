using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers.OpsManager;
using PcfAzureConfigurator.Helpers;

namespace PcfAzureConfigurator.Controllers
{
    [Produces("application/json")]
    [Route("api/v0/opsmanager/director")]
    public class DirectorController : Controller
    {
        private IDirectorHelper _directorHelper;

        public DirectorController(IDirectorHelper directorHelper)
        {
            _directorHelper = directorHelper;
        }

        [HttpGet]
        public async Task<JsonResult> Get(string opsManagerFqdn, string token)
        {
            JsonResult result;
            try
            {
                var properties = await _directorHelper.GetProperties(opsManagerFqdn, token);
                result = new JsonResult(new { result = properties });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { error = e.Response.Content });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
    }
}
