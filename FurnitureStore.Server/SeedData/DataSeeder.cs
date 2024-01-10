using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Repositories.Interfaces;

namespace FurnitureStore.Server.SeedData;

public class DataSeeder
{
    private readonly string _productsFilePath = "./SeedData/SampleData/products.json";
    private readonly string _ordersFilePath = "./SeedData/SampleData/orders.json";
    private readonly string _categoriesFilePath = "./SeedData/SampleData/categories.json";
    private readonly string _staffsFilePath = "./SeedData/SampleData/staffs.json";
    private readonly string _variationsFilePath = "./SeedData/SampleData/variations.json";

    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

    private readonly Container _productContainer;
    private readonly Container _orderContainer;
    private readonly Container _categoryContainer;
    private readonly Container _staffContainer;
    private readonly Container _variationContainer;

    public DataSeeder(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IStaffRepository staffRepository,
        IOrderRepository orderRepository,
        ILogger<DataSeeder> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _staffRepository = staffRepository;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task SeedDataAsync()
    {
        var productsJsonData = File.ReadAllText(_productsFilePath);
        var ordersJsonData = File.ReadAllText(_ordersFilePath);
        var categoriesJsonData = File.ReadAllText(_categoriesFilePath);
        var staffsJsonData = File.ReadAllText(_staffsFilePath);
        var variationsJsonData = File.ReadAllText(_variationsFilePath);


        // Seed products
        var productsItems = JsonConvert.DeserializeObject<List<ProductDocument>>(productsJsonData);

        if (productsItems != null)
        {
            foreach (var item in productsItems)
            {
                item.CreatedAt ??= DateTime.UtcNow;
                item.ModifiedAt ??= item.CreatedAt;

                 await _productContainer.UpsertItemAsync(
                     item: item,
                     partitionKey: new PartitionKey(item.Sku)
                 );
            }

            _logger.LogInformation("Populated product data");
        }

        // Seed orders
        var ordersItems = JsonConvert.DeserializeObject<List<OrderDocument>>(ordersJsonData);

        if (ordersItems != null)
        {
            foreach (var item in ordersItems)
            {
                item.CreatedAt = DateTime.UtcNow;
                item.ModifiedAt = item.CreatedAt;

                await _orderContainer.UpsertItemAsync(
                    item: item,
                    partitionKey: new PartitionKey(item.YearMonth)
                );
            }

            _logger.LogInformation("Populated order data");
        }

        // Seed categories
        var categoriesItems = JsonConvert.DeserializeObject<List<CategoryDocument>>(categoriesJsonData);

        if (categoriesItems != null)
        {
            foreach (var item in categoriesItems)
            {
                item.CreatedAt ??= DateTime.UtcNow;
                item.ModifiedAt ??= item.CreatedAt;

                await _categoryContainer.UpsertItemAsync(
                    item: item,
                    partitionKey: new PartitionKey(item.ParentPath)
                );
            }

            _logger.LogInformation("Populated category data");
        }

        // Seed staffs
        var staffsItems = JsonConvert.DeserializeObject<List<StaffDocument>>(staffsJsonData);

        if (staffsItems != null)
        {
            foreach (var item in staffsItems)
            {
                item.CreatedAt = DateTime.UtcNow;
                item.ModifiedAt = item.CreatedAt;
                
                await _staffContainer.UpsertItemAsync(
                    item: item,
                    partitionKey: new PartitionKey(item.StaffId)
                );
            }

            _logger.LogInformation("Populated staff data");
        }

        // Seed variations
        var variationsItems = JsonConvert.DeserializeObject<List<VariationDocument>>(variationsJsonData);

        if (variationsItems != null)
        {
            foreach (var item in variationsItems)
            {
                item.CreatedAt = DateTime.UtcNow;
                item.ModifiedAt = item.CreatedAt;
                
                await _variationContainer.UpsertItemAsync(
                    item: item,
                    partitionKey: new PartitionKey(item.VariationId)
                );
            }

            _logger.LogInformation("Populated variation data");
        }
    }
}
