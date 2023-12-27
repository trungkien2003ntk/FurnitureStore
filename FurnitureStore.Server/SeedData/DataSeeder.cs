using FurnitureStore.Server.IRepositories;
using FurnitureStore.Server.Models.Documents;

namespace FurnitureStore.Server.SeedData;

public class DataSeeder
{
    private readonly string _productsFilePath = "./SeedData/SampleData/products.json";
    private readonly string _ordersFilePath = "./SeedData/SampleData/orders.json";
    private readonly string _categoriesFilePath = "./SeedData/SampleData/categories.json";
    private readonly string _staffsFilePath = "./SeedData/SampleData/staffs.json";
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

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


        // Seed products
        var productsItems = JsonConvert.DeserializeObject<List<ProductDocument>>(productsJsonData);

        if (productsItems != null)
        {
            foreach (var item in productsItems)
            {
                await _productRepository.AddProductDocumentAsync(item);
            }

            _logger.LogInformation("Populated product data");
        }

        // Seed orders
        var ordersItems = JsonConvert.DeserializeObject<List<OrderDocument>>(ordersJsonData);

        if (ordersItems != null)
        {
            foreach (var item in ordersItems)
            {
                await _orderRepository.AddOrderDocumentAsync(item);
            }

            _logger.LogInformation("Populated order data");
        }

        // Seed categories
        var categoriesItems = JsonConvert.DeserializeObject<List<CategoryDocument>>(categoriesJsonData);

        if (categoriesItems != null)
        {
            foreach (var item in categoriesItems)
            {
                await _categoryRepository.AddCategoryDocumentAsync(item);
            }

            _logger.LogInformation("Populated category data");
        }

        // Seed staffs
        var staffsItems = JsonConvert.DeserializeObject<List<StaffDocument>>(staffsJsonData);

        if (staffsItems != null)
        {
            foreach (var item in staffsItems)
            {
                await _staffRepository.AddStaffDocumentAsync(item);
            }

            _logger.LogInformation("Populated staff data");
        }
    }
}
