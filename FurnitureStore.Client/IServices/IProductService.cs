using FurnitureStore.Shared;

namespace FurnitureStore.Client.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProduct();
    }
}
