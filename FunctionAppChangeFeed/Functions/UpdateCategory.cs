using FunctionAppChangeFeed.Models;
using FunctionAppChangeFeed.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionAppChangeFeed.Functions
{
    public class UpdateCategory
    {
        private readonly ILogger _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCategory(
            ILoggerFactory loggerFactory,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _logger = loggerFactory.CreateLogger<UpdateCategory>();
            this._productRepository = productRepository;
            this._categoryRepository = categoryRepository;
        }

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
}
