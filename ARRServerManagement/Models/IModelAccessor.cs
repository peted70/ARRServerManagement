using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ARRServerManagement.Models
{
    public interface IModelAccessor
    {
        Task<List<CloudBlobContainer>> GetContainersAsync();
        Task<List<CloudBlob>> ListAllBlobsAsync(string filter);
        Task<List<CloudBlob>> ListBlobsAsync(CloudBlobContainer container, string filter);
    }
}