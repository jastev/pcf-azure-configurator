using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PcfAzureConfigurator.Controllers.Azure
{
    [Produces("application/json")]
    [Route("api/Deployments")]
    public class DeploymentsController : Controller
    {
    }
}