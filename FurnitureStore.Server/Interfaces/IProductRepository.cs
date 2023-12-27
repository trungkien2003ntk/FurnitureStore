using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Shared;

namespace FurnitureStore.Server.Interfaces
{
    public interface IProductRepository
    {
        Task AddProductDocumentAsync(ProductDocument product);
        Task AddProductDTOAsync(ProductDTO productDTO);
        Task UpdateProductDTOAsync(ProductDTO productDTO);

        Task<string> GetNewProductIdAsync();
        Task<IEnumerable<ProductDocument>> GetProductDocumentsAsync();
        Task<IEnumerable<ProductDTO>> GetProductDTOsAsync();
        Task<IEnumerable<ProductDocument>> GetProductDocumentsInCategoryAsync(string categoryId);
        //Task<IEnumerable<ProductDTO>> GetProductDTOsInCategoryAsync(string categoryName);
        Task<ProductDTO?> GetProductDTOBySkuAsync(string sku);
        Task<ProductDTO?> GetProductDTOByIdAsync(string id);
        Task DeleteProductDTOAsync(string id);
    }
}
