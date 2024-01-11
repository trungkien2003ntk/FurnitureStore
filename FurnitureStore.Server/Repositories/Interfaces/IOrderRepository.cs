using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.BindingModels;

namespace FurnitureStore.Server.Repositories.Interfaces;

public interface IOrderRepository
{
    int TotalCount { get; }
    Task<IEnumerable<OrderDTO>?> GetOrderDTOsAsync(QueryParameters queryParameters, OrderFilterModel filter);
    Task<OrderDTO?> GetOrderDTOByIdAsync(string id);
    Task<OrderDTO?> AddOrderDTOAsync(OrderDTO orderDTO);
    Task UpdateOrderAsync(OrderDTO orderDTO);
}
