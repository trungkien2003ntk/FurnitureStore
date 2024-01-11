using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;

namespace FurnitureStore.Client.IServices
{
    public interface IProductService
    {
        Task<ProductDTO> AddProductAsync(ProductDTO productDTO);
        Task<bool> UpdateProductAsync(string productId, ProductDTO productDTO);
        Task<bool> DeleteProductAsync(string productId);
        Task<ProductResponse> GetProductDTOsByCategoryIdAndPageSizeAndPageNumber(string categoryId, int pageSize, int pageNumber);
    }
}
