using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using PcfAzureConfigurator.Helpers.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public class StorageHelper : IStorageHelper
    {
        private IHttpClient _httpClient;

        public StorageHelper(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StorageAccount[]> ListAccounts(string environmentName, string token, string subscriptionId, string resourceGroupName)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourceGroups/" + resourceGroupName + "/providers/Microsoft.Storage/storageAccounts?api-version=" + environment.ArmApiVersions.Storage);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var storageAccounts = JsonConvert.DeserializeObject<StorageAccountList>(responseContent).Value;

                return storageAccounts;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }

        public async Task CreateAccount(string environmentName, string token, string subscriptionId, string resourceGroupName, StorageAccount account)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourceGroups/" + resourceGroupName + "/providers/Microsoft.Storage/storageAccounts/" + account.Name + "?api-version=" + environment.ArmApiVersions.Storage);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpResponseException(response);
            }
        }

        public async Task<StorageAccount> GetAccount(string environmentName, string token, string subscriptionId, string resourceGroupName, string accountName)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourceGroups/" + resourceGroupName + "/providers/Microsoft.Storage/storageAccounts/" + accountName + "?api-version=" + environment.ArmApiVersions.Storage);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var storageAccount = JsonConvert.DeserializeObject<StorageAccount>(responseContent);

                return storageAccount;
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }

        public async Task<string> GetConnectionString(string environmentName, string token, string subscriptionId, string resourceGroupName, string accountName)
        {
            EnvironmentHelper.Environments.TryGetValue(environmentName, out AzureEnvironment environment);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, environment.Endpoints.ResourceManager + "/subscriptions/" + subscriptionId + "/resourceGroups/" + resourceGroupName + "/providers/Microsoft.Storage/storageAccounts/" + accountName + "/listKeys?api-version=" + environment.ArmApiVersions.Storage);
            requestMessage.Headers.Authorization = OauthToken.GetAuthenticationHeader(token);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var keys = JsonConvert.DeserializeObject<StorageAccountKeyList>(responseContent).Keys;
                // TODO need to check that the key has full permissions
                return String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", accountName, keys[0].Value);
            }
            else
            {
                throw new HttpResponseException(response);
            }
        }

        public async Task<CloudBlobContainer[]> ListContainers(string connectionString)
        {
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount account))
            {
                throw new Exception();  // TODO better exception
            }
            var blobClient = account.CreateCloudBlobClient();
            IEnumerable<CloudBlobContainer> containers = new List<CloudBlobContainer>();
            ContainerResultSegment segment;
            BlobContinuationToken continuation = null;
            do
            {
                segment = await blobClient.ListContainersSegmentedAsync(continuation);
                continuation = segment.ContinuationToken;
                containers = containers.Concat(segment.Results);
            } while (continuation != null);

            return containers.ToArray();
        }

        public async Task CreateContainer(string connectionString, string containerName)
        {
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount account))
            {
                throw new Exception();  // TODO better exception
            }
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateAsync();
        }

        public async Task<CloudBlob[]> ListBlobs(string connectionString, string containerName)
        {
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount account))
            {
                throw new Exception();  // TODO better exception
            }
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            IEnumerable<CloudBlob> blobs = new List<CloudBlob>();
            BlobResultSegment segment;
            BlobContinuationToken continuation = null;
            do
            {
                segment = await container.ListBlobsSegmentedAsync(continuation);
                continuation = segment.ContinuationToken;
                blobs = blobs.Concat(segment.Results.Select<IListBlobItem, CloudBlob>(blobItem => { return (CloudBlob)blobItem; }));
            } while (continuation != null);

            return blobs.ToArray();
        }

        public async Task CopyBlob(string connectionString, string destinationContainerName, string destinationBlobName, string sourceBlobUri)
        {
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount account))
            {
                throw new Exception();  // TODO better exception
            }
            var blobClient = account.CreateCloudBlobClient();
            await blobClient.GetContainerReference(destinationContainerName).GetBlobReference(destinationBlobName).StartCopyAsync(new Uri(sourceBlobUri));
        }

        public async Task<CloudBlob> GetBlob(string connectionString, string containerName, string blobName)
        {
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount account))
            {
                throw new Exception();  // TODO better exception
            }
            var blobClient = account.CreateCloudBlobClient();
            var blob = blobClient.GetContainerReference(containerName).GetBlobReference(blobName);
            await blob.FetchAttributesAsync();
            return blob;
        }

        public async Task<CloudTable[]> ListTables(string connectionString)
        {
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount account))
            {
                throw new Exception();  // TODO better exception
            }
            var tableClient = account.CreateCloudTableClient();
            IEnumerable<CloudTable> tables = new List<CloudTable>();
            TableResultSegment segment;
            TableContinuationToken continuation = null;
            do
            {
                segment = await tableClient.ListTablesSegmentedAsync(continuation);
                continuation = segment.ContinuationToken;
                tables = tables.Concat(segment.Results);
            } while (continuation != null);

            return tables.ToArray();
        }

        public async Task CreateTable(string connectionString, string tableName)
        {
            if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount account))
            {
                throw new Exception();  // TODO better exception
            }
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            await table.CreateAsync();
        }
    }
}

namespace PcfAzureConfigurator.Helpers.Azure.Storage
{
    public class StorageAccountList
    {
        public StorageAccount[] Value { get; set; }
    }

    public class StorageAccount
    {
        public string Kind { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public Sku Sku { get; set; }
        public AccountProperties Properties { get; set; }
    }

    public class Sku
    {
        public string Name { get; set; }
        public string Tier { get; set; }
    }

    public class AccountProperties
    {
        public string ProvisioningState { get; set; }
    }

    public class StorageAccountKeyList
    {
        public StorageAccountKey[] Keys { get; set; }
    }

    public class StorageAccountKey
    {
        public string KeyName { get; set; }
        public string Permissions { get; set; }
        public string Value { get; set; }
    }

    public class BlobContainer
    {
        public string Name { get; set; }
    }

    public class Blob
    {
        public string Name { get; set; }
        public BlobCopyState CopyState { get; set; }
    }

    public class BlobCopyState
    {
        public string Source { get; set; }
    }

    public class Table
    {
        public string Name { get; set; }
    }
}
