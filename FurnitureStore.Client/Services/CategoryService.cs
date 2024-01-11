using FurnitureStore.Client.IServices;
using FurnitureStore.Shared.DTOs;
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

        public async Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevel(int level)
        {
            string apiUrl = $"{GlobalConfig.CATEGORY_BASE_URL}?level={level}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryDTO>>(jsonResponse);
                return categories!;
            }
            return null!;
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParentId(string parentId)
        {
            string apiUrl = $"{GlobalConfig.CATEGORY_BASE_URL}?parentId={parentId}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryDTO>>(jsonResponse);
                return categories!;
            }
            return null!;
        }
    }
}
