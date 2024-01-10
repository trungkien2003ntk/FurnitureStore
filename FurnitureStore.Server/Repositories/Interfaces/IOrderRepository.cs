using FurnitureStore.Server.Models.Documents;

namespace FurnitureStore.Server.Repositories.Interfaces;

public interface IOrderRepository
{
    Task AddOrderDocumentAsync(OrderDocument item);
    Task AddOrderDTOAsync(OrderDTO orderDTO);
    Task DeleteOrderAsync(string id);
    Task<IEnumerable<OrderDTO>> GetOrderDTOsAsync();
    Task<string> GetNewOrderIdAsync();
    Task<OrderDTO> GetOrderDTOByIdAsync(string id);
    Task UpdateOrderAsync(OrderDTO orderDTO);
}
