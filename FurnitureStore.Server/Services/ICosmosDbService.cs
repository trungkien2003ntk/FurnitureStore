using FurnitureStore.Server.Models.Documents;

namespace FurnitureStore.Server.Services
{
    public interface ICosmosDbService
    {
        Task AddCartAsync(CartDocument item);
        Task AddCategoryAsync(CategoryDocument item);
        Task AddCustomerAsync(CustomerDocument item);
        Task AddOrderAsync(OrderDocument item);
        Task AddProductAsync(ProductDocument item);
        Task AddStaffAsync(StaffDocument item);
    }
}
