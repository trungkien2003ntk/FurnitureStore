using Azure;
using Azure.Storage.Blobs.Models;
using FurnitureStore.Server.Models.BindingModels;

namespace FurnitureStore.Server.Services
{
    public interface IFileService
    {
        Task<FileModel> UploadAsync(IFormFile fileModel);
        Task DeleteAsync(string blobName);
        //Task<string> UploadBase64(string base64Data);
    }
}
