namespace FunctionAppChangeFeed.IRepositories;

public interface IProductRepository
{
    Task ResetProductsCategoryBelongToCategoryIdAsync(string categoryId);
    Task UpdateProductCategoryBelongToCategoryIdAsync(string categoryId);
}
