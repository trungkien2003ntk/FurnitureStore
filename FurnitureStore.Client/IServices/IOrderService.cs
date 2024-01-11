using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Client.IServices
{
    public interface IOrderService
    {
        Task<OrderDTO> AddOrderAsync(OrderDTO orderDTO);
        Task<bool> DeleteOrderAsync(string orderId);
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<OrderDTO> GetOrderByIdAsync(string orderId);
        Task<bool> UpdateOrderAsync(string orderId, OrderDTO orderDTO);
    }
}
