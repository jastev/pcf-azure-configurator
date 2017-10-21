using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers
{
    public class HttpResponseException : Exception
    {
        public HttpResponseMessage Response { get; private set; }

        public HttpResponseException(HttpResponseMessage respoonse)
        {
            Response = respoonse;
        }

        new public string ToString()
        {
            return Response.Content.ToString();
        }
    }
}
