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
    [Route("api/v0/opsmanager/director")]
    public class DirectorController : Controller
    {
        private IDirectorHelper _directorHelper;

        public DirectorController(IDirectorHelper directorHelper)
        {
            _directorHelper = directorHelper;
        }

        [HttpGet]
        public async Task<DirectorProperties> Get(string opsManagerFqdn, string token)
        {
            var properties = await _directorHelper.GetProperties(opsManagerFqdn, token);
            return properties;
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
    }
}
