using FurnitureStore.Shared;

namespace FurnitureStore.Client.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetCategoryByLevel(int level);
    }
}
