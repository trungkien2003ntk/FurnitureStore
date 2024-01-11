using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Client.IServices
{
    public interface IProductService
    {
        Task<ProductDTO> AddProductAsync(ProductDTO productDTO);
        Task<bool> UpdateProductAsync(string productId, ProductDTO productDTO);
        Task<bool> DeleteProductAsync(string productId);
        Task<IEnumerable<ProductDTO>> GetProductDTOsByPageSizeAndPageNumber(int pageSize, int pageNumber);
    }
}
