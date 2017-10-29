using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using PcfAzureConfigurator.Helpers.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcfAzureConfigurator.Helpers.Azure
{
    public interface IStorageHelper
    {
        Task<StorageAccount[]> ListAccounts(string environmentName, string token, string subcriptionId, string resourceGroupName);
        Task CreateAccount(string environment, string token, string subscriptionId, string resourceGroupName, StorageAccount storageAccount);
        Task<StorageAccount> GetAccount(string environment, string token, string subscriptionId, string resourceGroupName, string storageAccountName);
        Task<string> GetConnectionString(string environment, string token, string subscriptionId, string resourceGroupName, string storageAccountName);
        Task<CloudBlobContainer[]> ListContainers(string connectionString);
        Task CreateContainer(string connectionString, string containerName);
        Task<CloudBlob[]> ListBlobs(string connectionString, string containerName);
        Task CopyBlob(string connectionString, string containerName, string blobName, string sourceUri);
        Task<CloudBlob> GetBlob(string connectionString, string containerName, string blobName);
        Task<CloudTable[]> ListTables(string connectionString);
        Task CreateTable(string connectionString, string tableName);
    }
}
