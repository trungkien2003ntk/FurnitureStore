using FunctionAppChangeFeed.IRepositories;
using FunctionAppChangeFeed.Models;
using FunctionAppChangeFeed.Utils;

namespace FunctionAppChangeFeed.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly Container _productContainer;
    private readonly Container _inventoryContainer;

    public ProductRepository(CosmosClient cosmosClient, ICategoryRepository categoryRepository)
    {
        var databaseName = cosmosClient.ClientOptions.ApplicationName;

        _productContainer = cosmosClient.GetContainer(databaseName, "products");
        _inventoryContainer = cosmosClient.GetContainer(databaseName, "inventory");
        _categoryRepository = categoryRepository;
    }


    public async Task<IEnumerable<Product>> GetProductDocumentsInCategoryAsync(string categoryId)
    {
        var queryDef = new QueryDefinition(
            query:
            "SELECT * " +
            "FROM products p " +
            "WHERE p.categoryId = @categoryId"
        ).WithParameter("@categoryId", categoryId);

        var results = await CosmosDbUtils.GetDocumentsByQueryDefinition<Product>(_productContainer, queryDef);

        return results;
    }

    public async Task ResetProductsCategoryBelongToCategoryIdAsync(string categoryId)
    {
        var productDocsBelongToCategory = await GetProductDocumentsInCategoryAsync(categoryId);
        await UpdateProductsBelongToCategoryId("cate00000", productDocsBelongToCategory);
    }

    public async Task UpdateProductCategoryBelongToCategoryIdAsync(string categoryId)
    {
        var productDocsBelongToCategory = await GetProductDocumentsInCategoryAsync(categoryId);
        await UpdateProductsBelongToCategoryId(categoryId, productDocsBelongToCategory);
    }

    private async Task UpdateProductsBelongToCategoryId(string categoryId, IEnumerable<Product> productDocsBelongToCategory)
    {
        var CategoryToUpdate = (await _categoryRepository.GetCategoryDocumentByIdAsync(categoryId))!;


        foreach (var productDoc in productDocsBelongToCategory)
        {
            Dictionary<string, string> productDetails = [];

            List<PatchOperation> operations =
            [
                PatchOperation.Replace("/categoryId", CategoryToUpdate.Id),
                PatchOperation.Replace("/categoryPath",CategoryToUpdate.CategoryPath)
            ];

            await _productContainer.PatchItemAsync<dynamic>(productDoc.Id, new PartitionKey(productDoc.Sku), operations);
        }
    }
}
