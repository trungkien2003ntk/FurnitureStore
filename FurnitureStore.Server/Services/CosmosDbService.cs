using FurnitureStore.Server.Models.Documents;
using Microsoft.Azure.Cosmos;

namespace FurnitureStore.Server.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Database _database;
        private Container _productContainer;
        private Container _cartContainer;
        private Container _staffContainer;
        private Container _categoryContainer;
        private Container _orderContainer;
        private Container _customerContainer;

        public CosmosDbService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            //var cosmosClient = new CosmosClient(configuration["CosmosDbSettings:EndpointUri"], configuration["CosmosDbSettings:PrimaryKey"]);

            _database = cosmosClient.GetDatabase("FurnitureStoreDb");
            _productContainer = _database.GetContainer("products");
            _cartContainer = _database.GetContainer("carts");
            _staffContainer = _database.GetContainer("staffs");
            _categoryContainer = _database.GetContainer("categories");
            _orderContainer = _database.GetContainer("orders");
            _customerContainer = _database.GetContainer("customers");
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
