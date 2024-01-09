using FurnitureStore.Shared;

namespace FurnitureStore.Client.IServices
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProduct();
    }
}
