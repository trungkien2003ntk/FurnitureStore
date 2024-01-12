using FurnitureStore.Shared;
using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Client.IServices.Customer
{
    public interface IProductService
    {
         Task<IEnumerable<ProductDTO>> GetLatestProducts();
         Task<ProductDTO> GetProductById(string ProductId);
         Task<IEnumerable<ProductDTO>> GetProductListByProductIdList(List<string>listProductId);
         Task<IEnumerable<ProductDTO>> GetProductsByVariationId(string id);
         Task<IEnumerable<ProductDTO>> GetProductsByCategoryId(string id);
    }
}