using FurnitureStore.Client.IServices;
using FurnitureStore.Shared.DTOs;
using Newtonsoft.Json;
using System.Text;

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
            string apiUrl = $"{GlobalConfig.CATEGORY_BASE_URL}level/{level}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryDTO>>(jsonResponse);
                return categories!;
            }
            return null!;
        }

        public async Task<CategoryDTO> GetCategoryDTOsById(string id)
        {
            string apiUrl = $"{GlobalConfig.CATEGORY_BASE_URL}{id}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<CategoryDTO>(jsonResponse);
                return categories!;
            }
            return null!;
        }

        public async Task<CategoryDTO> AddCategory(CategoryDTO category)
        {
            string apiUrl = GlobalConfig.CATEGORY_BASE_URL;

            var json = JsonConvert.SerializeObject(category);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(new Uri(apiUrl), content);

            if (response.IsSuccessStatusCode)
            {
                return category;
            }

            return null!;
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParent(string parent)
        {
            string apiUrl = $"{GlobalConfig.CATEGORY_BASE_URL}parent/{parent}";

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
