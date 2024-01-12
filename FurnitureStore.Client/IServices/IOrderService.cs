using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;

namespace FurnitureStore.Client.IServices
{
    public interface IOrderService
    {
        Task<OrderDTO?> AddOrderAsync(OrderDTO orderDTO);
        Task<OrderResponse?> GetOrderResponseAsync(string? status = null);
        Task<OrderDTO?> GetOrderByIdAsync(string orderId);
        Task<bool> UpdateOrderAsync(string orderId, OrderDTO orderDTO);
    }
}
