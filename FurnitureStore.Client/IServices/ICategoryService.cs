using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;

namespace FurnitureStore.Client.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoryResponsesWithNestedResult();
        Task<CategoryDTO> AddCategoryAsync(CategoryDTO categoryDTO);
        Task<bool> DeleteCategoryDTOAsync(string categoryId);
        Task<bool> UpdateCategoryDTOAsync(string categoryId, CategoryDTO categoryDTO);
        Task<CategoryDTO> GetCategoryDTOByIdAsync(string categoryId);
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevelAsync(int level);
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParentIdAsync(string parentId);
    }
}
