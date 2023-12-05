using FurnitureStore.Server.Models.Documents;
using Microsoft.Azure.Cosmos;

namespace FurnitureStore.Server.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _productContainer;
        private readonly Container _cartContainer;
        private readonly Container _staffContainer;
        private readonly Container _categoryContainer;
        private readonly Container _orderContainer;
        private readonly Container _customerContainer;

        public CosmosDbService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            var database = cosmosClient.GetDatabase(configuration["CosmosDbSettings:DatabaseName"]);
            _productContainer = database.GetContainer("products");
            _cartContainer = database.GetContainer("carts");
            _staffContainer = database.GetContainer("staffs");
            _categoryContainer = database.GetContainer("categories");
            _orderContainer = database.GetContainer("orders");
            _customerContainer = database.GetContainer("customers");
        }

        public async Task AddCategoryAsync(CategoryDocument item)
        {
            await _categoryContainer.CreateItemAsync(item, new PartitionKey(item.Parent));
        }

        public async Task AddCustomerAsync(CustomerDocument item)
        {
            await _customerContainer.CreateItemAsync(item, new PartitionKey(item.CustomerId));
        }

        public async Task AddOrderAsync(OrderDocument item)
        {
            await _orderContainer.CreateItemAsync(item, new PartitionKey(item.CustomerId));
        }

        public async Task AddProductAsync(ProductDocument item)
        {
            await _productContainer.CreateItemAsync(item, new PartitionKey(item.ProductId));
        }

        public async Task AddStaffAsync(StaffDocument item)
        {
            await _staffContainer.CreateItemAsync(item, new PartitionKey(item.StaffId));
        }

        public async Task AddCartAsync(CartDocument item)
        {
            await _cartContainer.CreateItemAsync(item, new PartitionKey(item.CustomerId));
        }
    }
}
