using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;
using Microsoft.AspNetCore.Components.Forms;

namespace FurnitureStore.Client.Admin.IServices
{
    public interface IProductService
    {
        Task<ProductDTO> AddProductAsync(ProductDTO productDTO);
        Task<bool> UpdateProductAsync(string productId, ProductDTO productDTO);
        Task<bool> DeleteProductAsync(string productId);
        Task<ProductDTO?> GetProductDTOByIdAsync(string productId);
        Task<ProductResponse?> GetProductResponseAsync(List<string>? categoryIds = null, string? variationId = null, int? pageSize = null, int? pageNumber = null, List<string>? priceRangeStrings = null);
        Task<ProductResponse?> SearchProductAsync(string queryText, int? pageSize = null, int? pageNumber = null, List<string>? priceRangeStrings = null);
        Task<ImageResponse?> UploadImageAsync(List<IBrowserFile> browserFiles);
    }
}
