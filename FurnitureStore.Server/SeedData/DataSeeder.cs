using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Services;
using Newtonsoft.Json;

namespace FurnitureStore.Server.SeedData
{
    public class DataSeeder
    {
        private readonly string _cartsFilePath = "./SeedData/SampleData/carts.json";
        private readonly string _customersFilePath = "./SeedData/SampleData/customers.json";
        private readonly string _productsFilePath = "./SeedData/SampleData/products.json";
        private readonly string _ordersFilePath = "./SeedData/SampleData/orders.json";
        private readonly string _categoriesFilePath = "./SeedData/SampleData/categories.json";
        private readonly string _staffsFilePath = "./SeedData/SampleData/staffs.json";


        private readonly ICosmosDbService _cosmosDbService;

        public DataSeeder(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task SeedDataAsync()
        {
            var cartsJsonData = File.ReadAllText(_cartsFilePath);
            var customersJsonData = File.ReadAllText(_customersFilePath);
            var productsJsonData = File.ReadAllText(_productsFilePath);
            var ordersJsonData = File.ReadAllText(_ordersFilePath);
            var categoriesJsonData = File.ReadAllText(_categoriesFilePath);
            var staffsJsonData = File.ReadAllText(_staffsFilePath);

            
            // Seed Carts
            var cartItems = JsonConvert.DeserializeObject<List<CartDocument>>(cartsJsonData);

            if (cartItems != null)
            {
                foreach (var item in cartItems)
                {
                    await _cosmosDbService.AddCartAsync(item);
                }
            }

            // Seed customers
            var customersItems = JsonConvert.DeserializeObject<List<CustomerDocument>>(customersJsonData);

            if (customersItems != null)
            {
                foreach (var item in customersItems)
                {
                    await _cosmosDbService.AddCustomerAsync(item);
                }
            }

            // Seed products
            var productsItems = JsonConvert.DeserializeObject<List<ProductDocument>>(productsJsonData);

            if (productsItems != null)
            {
                foreach (var item in productsItems)
                {
                    await _cosmosDbService.AddProductAsync(item);
                }
            }

            // Seed orders
            var ordersItems = JsonConvert.DeserializeObject<List<OrderDocument>>(ordersJsonData);

            if (ordersItems != null)
            {
                foreach (var item in ordersItems)
                {
                    await _cosmosDbService.AddOrderAsync(item);
                }
            }

            // Seed categories
            var categoriesItems = JsonConvert.DeserializeObject<List<CategoryDocument>>(categoriesJsonData);

            if (categoriesItems != null)
            {
                foreach (var item in categoriesItems)
                {
                    await _cosmosDbService.AddCategoryAsync(item);
                }
            }

            // Seed staffs
            var staffsItems = JsonConvert.DeserializeObject<List<StaffDocument>>(staffsJsonData);

            if (staffsItems != null)
            {
                foreach (var item in staffsItems)
                {
                    await _cosmosDbService.AddStaffAsync(item);
                }
            }
        }
    }
}
