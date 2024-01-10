using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.Documents;

namespace FurnitureStore.Server.Repositories.Interfaces;

public interface IProductRepository
{
    Task AddProductDocumentAsync(ProductDocument product);
    Task AddProductDTOAsync(ProductDTO productDTO);
    Task UpdateProductDTOAsync(ProductDTO productDTO);

    Task<string> GetNewProductIdAsync();
    Task<IEnumerable<ProductDocument>> GetProductDocumentsAsync();
    Task<IEnumerable<ProductDTO>> GetProductDTOsAsync(QueryParameters queryParameters, ProductFilterModel filter);
    Task<IEnumerable<ProductDTO>> GetProductDTOsByVariationIdAsync(string variationId);
    Task<IEnumerable<ProductDocument>> GetProductDocumentsInCategoryAsync(string categoryId);
    //Task<IEnumerable<ProductDTO>> GetProductDTOsInCategoryAsync(string categoryName);
    Task<ProductDTO?> GetProductDTOBySkuAsync(string sku);
    Task<ProductDTO?> GetProductDTOByIdAsync(string id);
    Task DeleteProductDTOAsync(string id);
}
