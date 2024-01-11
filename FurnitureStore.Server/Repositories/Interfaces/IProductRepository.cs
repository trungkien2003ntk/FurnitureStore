using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.Documents;

namespace FurnitureStore.Server.Repositories.Interfaces;

public interface IProductRepository
{
    int TotalCount { get; }

    Task<IEnumerable<ProductDTO>> GetProductDTOsAsync(QueryParameters queryParameters, ProductFilterModel filter);
    Task<ProductDTO?> GetProductDTOByIdAsync(string id);
    Task<ProductDTO?> AddProductDTOAsync(ProductDTO productDTO);
    Task UpdateProductDTOAsync(ProductDTO productDTO);
    Task DeleteProductDTOAsync(string id);
}
