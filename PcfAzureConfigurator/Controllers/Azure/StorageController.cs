using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PcfAzureConfigurator.Helpers.Azure;
using PcfAzureConfigurator.Helpers.Azure.Storage;
using PcfAzureConfigurator.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace PcfAzureConfigurator.Controllers.Azure
{
    [Route("api/v0/azure/{environment}/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/storageaccounts")]
    [Produces("application/json")]
    public class StorageController : Controller
    {
        private IStorageHelper _storageHelper;

        public StorageController(IStorageHelper storageHelper)
        {
            _storageHelper = storageHelper;
        }

        [HttpGet]
        public async Task<JsonResult> ListAccounts(string environment, string subscriptionId, string resourceGroupName)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var accounts = await _storageHelper.ListAccounts(environment, token, subscriptionId, resourceGroupName);
                result = new JsonResult(new { result = accounts });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{storageAccountName}")]
        [HttpPut]
        public async Task<JsonResult> CreateAccount(string environment, string subscriptionId, string resourceGroupName, string storageAccountName, [FromBody] StorageAccount storageAccount)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                await _storageHelper.CreateAccount(environment, token, subscriptionId, resourceGroupName, storageAccount);
                result = new JsonResult(new { result = true });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{storageAccountName}")]
        [HttpGet]
        public async Task<JsonResult> GetAccount(string environment, string subscriptionId, string resourceGroupName, string storageAccountName)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var account = await _storageHelper.GetAccount(environment, token, subscriptionId, resourceGroupName, storageAccountName);
                result = new JsonResult(new { result = account });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{storageAccountName}/connectionstring")]
        [HttpGet]
        public async Task<JsonResult> GetConnectionString(string environment, string subscriptionId, string resourceGroupName, string storageAccountName)
        {
            var token = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var connectionString = await _storageHelper.GetConnectionString(environment, token, subscriptionId, resourceGroupName, storageAccountName);
                result = new JsonResult(new { result = connectionString });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        // For all of the below, the Authorization header needs to contain the storage account connection string obtained above, rather than the ARM token

        [Route("{storageAccountName}/containers")]
        [HttpGet]
        public async Task<JsonResult>ListContainers(string environment, string subscriptionId, string resourceGroupName, string storageAccountName)
        {
            var connectionString = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                var containers = await _storageHelper.ListContainers(connectionString);
                result = new JsonResult(new { result = containers });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{storageAccountName}/containers/{containerName}")]
        [HttpPut]
        public async Task<JsonResult> CreateContainer(string environment, string subscriptionId, string resourceGroupName, string storageAccountName, string containerName)
        {
            var connectionString = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                await _storageHelper.CreateContainer(connectionString, containerName);
                result = new JsonResult(new { result = true });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{storageAccountName}/containers/{containerName}/blobs")]
        [HttpGet]
        public async Task<JsonResult> ListBlobs(string environment, string subscriptionId, string resourceGroupName, string storageAccountName, string containerName)
        {
            {
                var connectionString = OauthToken.GetToken(HttpContext.Request.Headers);

                JsonResult result;
                try
                {
                    var blobs = await _storageHelper.ListBlobs(connectionString, containerName);
                    result = new JsonResult(new { result = blobs });
                }
                catch (HttpResponseException e)
                {
                    result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                    result.StatusCode = (int)e.Response.StatusCode;
                }
                return result;
            }
        }

        [Route("{storageAccountName}/containers/{containerName}/blobs/{blobName}")]
        [HttpPut]
        public async Task<JsonResult> CopyBlob(string environment, string subscriptionId, string resourceGroupName, string storageAccountName, string containerName, string blobName, [FromBody] BlobCopySource source)
        {
            var connectionString = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                await _storageHelper.CopyBlob(connectionString, containerName, blobName, source.Uri);
                result = new JsonResult(new { result = true });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }

        [Route("{storageAccountName}/containers/{containerName}/blobs/{blobName}")]
        [HttpGet]
        public async Task<JsonResult> GetBlob(string environment, string subscriptionId, string resourceGroupName, string storageAccountName, string containerName, string blobName)
        {
            {
                var connectionString = OauthToken.GetToken(HttpContext.Request.Headers);

                JsonResult result;
                try
                {
                    var copyState =  await _storageHelper.GetBlob(connectionString, containerName, blobName);
                    result = new JsonResult(new { result = copyState });
                }
                catch (HttpResponseException e)
                {
                    result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                    result.StatusCode = (int)e.Response.StatusCode;
                }
                return result;
            }
        }

        [Route("{storageAccountName}/tables")]
        [HttpGet]
        public async Task<JsonResult> ListTables(string environment, string subscriptionId, string resourceGroupName, string storageAccountName)
        {
            {
                var connectionString = OauthToken.GetToken(HttpContext.Request.Headers);

                JsonResult result;
                try
                {
                    var tables = await _storageHelper.ListTables(connectionString);
                    result = new JsonResult(new { result = tables });
                }
                catch (HttpResponseException e)
                {
                    result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                    result.StatusCode = (int)e.Response.StatusCode;
                }
                return result;
            }
        }

        [Route("{storageAccountName}/tables/{tableName}")]
        [HttpPut]
        public async Task<JsonResult> CreateTable(string environment, string subscriptionId, string resourceGroupName, string storageAccountName, string tableName)
        {
            var connectionString = OauthToken.GetToken(HttpContext.Request.Headers);

            JsonResult result;
            try
            {
                await _storageHelper.CreateTable(connectionString, tableName);
                result = new JsonResult(new { result = true });
            }
            catch (HttpResponseException e)
            {
                result = new JsonResult(new { result = new { code = (int)e.Response.StatusCode, data = await e.Response.Content.ReadAsStringAsync() } });
                result.StatusCode = (int)e.Response.StatusCode;
            }
            return result;
        }
    }

    public class BlobCopySource
    {
        public string Uri { get; set; }
    }
}