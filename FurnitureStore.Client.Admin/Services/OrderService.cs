using FurnitureStore.Client.Admin.IServices;
using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace FurnitureStore.Client.Admin.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OrderDTO?> AddOrderAsync(OrderDTO orderDTO)
        {
            string apiUrl = GlobalConfig.ORDER_BASE_URL;

            var json = JsonConvert.SerializeObject(orderDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(new Uri(apiUrl), content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var returnedOrderDTO = JsonConvert.DeserializeObject<OrderDTO>(responseContent);
                return returnedOrderDTO;
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }

            return null;
        }

        public async Task<OrderResponse?> GetOrderResponseAsync(string? status = null)
        {
            string apiUrl = GlobalConfig.ORDER_BASE_URL + $"{(status == null? $"?status={status}" : "")}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var orderResponse= JsonConvert.DeserializeObject<OrderResponse>(jsonResponse);
                return orderResponse;
            }

            return null;
        }

        public async Task<OrderDTO?> GetOrderByIdAsync(string orderId)
        {
            string apiUrl = $"{GlobalConfig.ORDER_BASE_URL}/{orderId}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string? jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OrderDTO>(jsonResponse!)!;
            }

            return null;
        }

        public async Task<bool> UpdateOrderAsync(string orderId, OrderDTO orderDTO)
        {
            string apiUrl = $"{GlobalConfig.ORDER_BASE_URL}/{orderId}";

            var json = JsonConvert.SerializeObject(orderDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(new Uri(apiUrl), content);

            return response.IsSuccessStatusCode;
        }
    }
}
