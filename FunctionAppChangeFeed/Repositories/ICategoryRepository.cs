using FunctionAppChangeFeed.Models;

namespace FunctionAppChangeFeed.Repositories
{
    public interface ICategoryRepository
    {
        Task DeleteCategoryAsync(Category updatedCategory);
        Task<Category> GetCategoryDocumentByIdAsync(string categoryId);
    }
}
