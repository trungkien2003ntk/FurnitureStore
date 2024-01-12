using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;

namespace FurnitureStore.Client.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoryResponsesWithNestedResult();
        Task<CategoryDTO> GetCategoryDTOsById(string id);
        Task<CategoryDTO> AddCategoryAsync(CategoryDTO categoryDTO);
        Task<bool> DeleteCategoryDTOAsync(string categoryId);
        Task<bool> UpdateCategoryDTOAsync(string categoryId, CategoryDTO categoryDTO);
        Task<CategoryDTO> GetCategoryDTOByIdAsync(string categoryId);
        Task<IEnumerable<CategoryResponse>?> GetCategoryDTOsByLevelAsync(int level);
        Task<IEnumerable<CategoryResponse>?> GetCategoryDTOsByParentIdAsync(string parentId);
    }
}
