using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ARRServerManagement.Models
{
    public interface IModelAccessor
    {
        Task<List<CloudBlobContainer>> GetContainersAsync();
        Task<List<CloudBlob>> ListBlobsAsync(string filter);
    }
}