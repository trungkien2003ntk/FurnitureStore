using FurnitureStore.Client.IServices;
using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;
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

        public async Task<CategoryDTO> AddCategoryAsync(CategoryDTO categoryDTO)
        {
            string apiUrl = GlobalConfig.CATEGORY_BASE_URL;

            var json = JsonConvert.SerializeObject(categoryDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(new Uri(apiUrl), content);

            if (response.IsSuccessStatusCode)
            {
                return categoryDTO;
            }

            return null!;
        }

        public async Task<bool> DeleteCategoryDTOAsync(string categoryId)
        {
            string apiUrl = $"{GlobalConfig.CATEGORY_BASE_URL}/{categoryId}";
            var response = await _httpClient.DeleteAsync(new Uri(apiUrl)).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevelAsync(int level)
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

        public async Task<CategoryDTO> GetCategoryDTOsById(string id)
        {
            string apiUrl = $"{GlobalConfig.CATEGORY_BASE_URL}/{id}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<CategoryDTO>(jsonResponse);
                return categories!;
            }
            return null!;
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParentIdAsync(string parentId)
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

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoryResponsesWithNestedResult()
        {
            string apiUrl = $"{GlobalConfig.CATEGORY_BASE_URL}?useNestedResult=true";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var categoryResponse = JsonConvert.DeserializeObject<IEnumerable<CategoryResponse>>(jsonResponse);
                return categoryResponse!;
            }
            return null!;
        }

        public async Task<bool> UpdateCategoryDTOAsync(string categoryId, CategoryDTO categoryDTO)
        {
            string apiUrl = $"{GlobalConfig.CATEGORY_BASE_URL}/{categoryId}";

            var json = JsonConvert.SerializeObject(categoryDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(new Uri(apiUrl), content);

            return response.IsSuccessStatusCode;
        }
    }
}
