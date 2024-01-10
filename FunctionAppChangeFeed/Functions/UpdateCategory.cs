using FunctionAppChangeFeed.IRepositories;
using FunctionAppChangeFeed.Models;

namespace FunctionAppChangeFeed.Functions;

public class UpdateCategory(
    ILoggerFactory loggerFactory,
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<UpdateCategory>();
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    [Function("UpdateCategoryFunction")]
    public async Task Run([CosmosDBTrigger(
        databaseName: "FurnitureStoreDb",
        containerName: "categories",
        Connection = "CosmosDbConnectionString",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Category> input)
    {
        try
        {
            if (input != null && input.Count > 0)
            {
                foreach (var updatedCategory in input)
                {
                    if (updatedCategory == null)
                        continue;

                    // Category is going to be deleted
                    if (updatedCategory.IsDeleted && updatedCategory.TTL == -1)
                    {
                        _logger.LogInformation($"Deleting Category {updatedCategory.Name}");
                        await _productRepository.ResetProductsCategoryBelongToCategoryIdAsync(updatedCategory.CategoryId);
                        await _categoryRepository.DeleteCategoryAsync(updatedCategory);
                    }
                    else if (!updatedCategory.IsDeleted)// Category is updated
                    {
                        _logger.LogInformation($"Updating Category {updatedCategory.Name}");
                        await _productRepository.UpdateProductCategoryBelongToCategoryIdAsync(updatedCategory.CategoryId);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.Message);
        }
    }
}
