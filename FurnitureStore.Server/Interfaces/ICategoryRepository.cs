using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Shared;

namespace FurnitureStore.Server.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddCategoryDocumentAsync(CategoryDocument item);
        Task UpdateCategoryAsync(CategoryDTO item);
        Task AddCategoryDTOAsync(CategoryDTO item);

        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsAsync();
        Task<CategoryDTO> GetCategoryDTOByIdAsync(string id);
        Task DeleteCategoryAsync(string id);
        Task<string> GetNewCategoryIdAsync();
    }
}
