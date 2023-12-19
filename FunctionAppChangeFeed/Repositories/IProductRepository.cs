namespace FunctionAppChangeFeed.Repositories
{
    public interface IProductRepository
    {
        Task ResetProductsCategoryBelongToCategoryIdAsync(string categoryId);
        Task UpdateProductCategoryBelongToCategoryIdAsync(string categoryId);
    }
}
