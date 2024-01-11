using FurnitureStore.Client.IServices;
using FurnitureStore.Shared.DTOs;
using Newtonsoft.Json;
using System.Text;

namespace FurnitureStore.Client.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<OrderDTO> AddOrderAsync(OrderDTO orderDTO)
        {
            string apiUrl = GlobalConfig.ORDER_BASE_URL;

            var json = JsonConvert.SerializeObject(orderDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(new Uri(apiUrl), content);

            if (response.IsSuccessStatusCode)
            {
                return orderDTO;
            }

            return null!;
        }

        public async Task<bool> DeleteOrderAsync(string orderId)
        {
            string apiUrl = $"{GlobalConfig.ORDER_BASE_URL}/{orderId}";
            var response = await _httpClient.DeleteAsync(new Uri(apiUrl)).ConfigureAwait(false);

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            string apiUrl = GlobalConfig.ORDER_BASE_URL;

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<OrderDTO>>(jsonResponse);
                return orders!;
            }
            return null!;
        }

        public async Task<OrderDTO> GetOrderByIdAsync(string orderId)
        {
            string apiUrl = $"{GlobalConfig.ORDER_BASE_URL}/{orderId}";

            var response = await _httpClient.GetAsync(new Uri(apiUrl));
            if (response.IsSuccessStatusCode)
            {
                string? jsonResponse = await response.Content.ReadAsStringAsync();
                return (JsonConvert.DeserializeObject<OrderDTO>(jsonResponse!))!;
            }

            return null!;
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
