﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers
{
    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message);
    }
}
