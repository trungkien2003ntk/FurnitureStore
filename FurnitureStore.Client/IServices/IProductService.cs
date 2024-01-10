using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Client.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProduct();
    }
}
