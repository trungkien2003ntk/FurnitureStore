using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;

namespace FurnitureStore.Client.IServices
{
    public interface IProductService
    {
        Task<ProductDTO> AddProductAsync(ProductDTO productDTO);
        Task<bool> UpdateProductAsync(string productId, ProductDTO productDTO);
        Task<bool> DeleteProductAsync(string productId);
        Task<ProductDTO> GetProductDTOByIdAsync(string productId);
        Task<ProductResponse> GetProductDTOsAsync(List<string>? categoryIds = null, string? variationId = null, int? pageSize = null, int? pageNumber = null, List<string>? priceRangeStrings = null);
        Task<ProductResponse> SearchProductAsync(string queryText, int? pageSize = null, int? pageNumber = null, List<string>? priceRangeStrings = null);
    }
}
