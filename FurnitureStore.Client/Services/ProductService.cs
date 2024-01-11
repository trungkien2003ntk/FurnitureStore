﻿using FurnitureStore.Client.IServices;
using FurnitureStore.Shared.DTOs;
using Newtonsoft.Json;
using System.Text;

namespace FurnitureStore.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductDTO> AddProductAsync(ProductDTO productDTO)
        {
            string apiUrl = GlobalConfig.PRODUCT_BASE_URL;

            var json = JsonConvert.SerializeObject(productDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(new Uri(apiUrl), content);

            if (response.IsSuccessStatusCode)
            {
                return productDTO;
            }

            return null!;
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            string apiUrl = $"{GlobalConfig.PRODUCT_BASE_URL}/{productId}";
            var response = await _httpClient.DeleteAsync(new Uri(apiUrl)).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductDTOsByPageSizeAndPageNumber(int pageSize, int pageNumber)
        {
            string apiUrl = $"{GlobalConfig.PRODUCT_BASE_URL}?pageSize={pageSize}&pageNumber={pageNumber}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductDTO>>(jsonResponse);
                return products!;
            }
            return null!;
        }

        public async Task<bool> UpdateProductAsync(string productId, ProductDTO productDTO)
        {
            string apiUrl = $"{GlobalConfig.PRODUCT_BASE_URL}/{productId}";

            var json = JsonConvert.SerializeObject(productDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(new Uri(apiUrl), content);

            return response.IsSuccessStatusCode;
        }
    }
}
