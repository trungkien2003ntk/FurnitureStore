using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Client.IServices
{
    public interface ICategoryService
    {
        Task<bool> DeleteCategoryDTOAsync(string categoryId);
        Task<bool> UpdateCategoryDTOAsync(string categoryId, CategoryDTO categoryDTO);
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevelAsync(int level);
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParentIdAsync(string parentId);
    }
}
