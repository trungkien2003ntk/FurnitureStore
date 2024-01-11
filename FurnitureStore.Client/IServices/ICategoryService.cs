using FurnitureStore.Shared;

namespace FurnitureStore.Client.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevel(int level);
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParent(string parent);
    }
}
