using FurnitureStore.Client.IServices.Customer;
using FurnitureStore.Shared;
using FurnitureStore.Shared.DTOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;

namespace FurnitureStore.Client.Services.Customer
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<ProductDTO>> GetLatestProducts()
        {
        
            string apiUrl = "https://localhost:7007/api/Products?pageSize=35&pageNumber=1";
            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<List<ProductDTO>>(jsonResponse);
                return product!;
            }
            return null!;

        }
        public async Task<ProductDTO> GetProductById (string ProductId)
        {
            string apiUrl = "https://localhost:7007/api/Products/" + ProductId;
            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductDTO>(jsonResponse);
                return product!;
            }
            return null!;
        }
        public async Task<IEnumerable<ProductDTO>> GetProductListByProductIdList(List<string> listProductId)
        {
            List<ProductDTO> result= new List<ProductDTO>();
            for (int i = 0; i < listProductId.Count; i++)
            {
                string apiUrl = "https://localhost:7007/api/Products/" + listProductId[i];
                var response = await _httpClient.GetAsync(new Uri(apiUrl));
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ProductDTO>(jsonResponse);
                    if (product != null)
                    result.Add(product);
                }
            }
            return result!;
        }
    }
}
