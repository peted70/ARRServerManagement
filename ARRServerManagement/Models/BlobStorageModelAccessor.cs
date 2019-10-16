using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARRServerManagement.Models
{
    public class BlobStorageModelAccessor : IModelAccessor
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;

        public BlobStorageModelAccessor(IConfiguration config)
        {
            var storageConnectionString = config.GetValue<string>("Azure:StorageConnectionString");
            if (string.IsNullOrEmpty(storageConnectionString))
                throw new MissingOrInvalidConnectionStringException("Azure Blob Storage Connection String Null or Empty");

            if (!CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount _storageAccount))
                throw new MissingOrInvalidConnectionStringException("Azure Blob Storage Connection String Invalid");

            _blobClient = _storageAccount.CreateCloudBlobClient();
        }

        public async Task<List<CloudBlob>> ListBlobsAsync(string filter)
        {
            BlobContinuationToken continuationToken = null;
            List<CloudBlob> results = new List<CloudBlob>();
            do
            {
                var response = await _blobClient.ListBlobsSegmentedAsync("", true, BlobListingDetails.None, 
                    500, continuationToken, null, null);

                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results
                    .Where(b => b is CloudBlob && ((CloudBlob)b).Name.EndsWith(filter))
                    .Cast<CloudBlob>());
            }
            while (continuationToken != null);
            return results;
        }

        public async Task<List<CloudBlobContainer>> GetContainersAsync()
        {
            BlobContinuationToken continuationToken = null;
            List<CloudBlobContainer> results = new List<CloudBlobContainer>();
            do
            {
                var response = await _blobClient.ListContainersSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results;
        }
    }
}
