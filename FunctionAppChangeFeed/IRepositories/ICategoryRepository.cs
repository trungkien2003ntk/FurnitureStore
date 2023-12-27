using FunctionAppChangeFeed.Models;

namespace FunctionAppChangeFeed.IRepositories;

public interface ICategoryRepository
{
    Task DeleteCategoryAsync(Category updatedCategory);
    Task<Category> GetCategoryDocumentByIdAsync(string categoryId);
}
