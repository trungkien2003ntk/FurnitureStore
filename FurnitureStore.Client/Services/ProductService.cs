using FurnitureStore.Client.IServices;
using FurnitureStore.Shared;
using Newtonsoft.Json;
namespace FurnitureStore.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<ProductDTO>> GetAllProduct()
        {
            string apiUrl = $"{GlobalConfig.PRODUCT_BASE_URL}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<List<ProductDTO>>(jsonResponse);
                return product;
            }

            return null;
        }
    }
}
