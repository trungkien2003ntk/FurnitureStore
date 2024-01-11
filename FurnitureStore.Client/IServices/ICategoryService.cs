using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Client.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevel(int level);
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParent(string parent);
        Task<CategoryDTO> GetCategoryDTOsById(string id);
        Task<CategoryDTO> AddCategory(CategoryDTO category);
    }
}
