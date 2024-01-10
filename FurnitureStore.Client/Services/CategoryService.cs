using FurnitureStore.Client.IServices;
using FurnitureStore.Shared;
using Newtonsoft.Json;
namespace FurnitureStore.Client.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<CategoryDTO>> GetCategoryByLevel(int level)
        {
            var categoryList = new List<CategoryDTO>
            {
                new CategoryDTO { Id = "1", CategoryId = "C1", Name = "Furniture", Level = 1 },
                new CategoryDTO { Id = "2", CategoryId = "C2", Name = "Furniture2", Level = 1 },
                new CategoryDTO { Id = "3", CategoryId = "C3", Name = "Electronics", Level = 1 },
                new CategoryDTO { Id = "4", CategoryId = "C4", Name = "Appliances", Level = 1 },
                new CategoryDTO { Id = "5", CategoryId = "C5", Name = "Clothing", Level = 1 },
            };
            return categoryList!;
        }
    }
}
