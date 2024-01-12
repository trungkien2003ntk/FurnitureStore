using FurnitureStore.Client.Admin.IServices;
using FurnitureStore.Shared.DTOs;
using Newtonsoft.Json;
using System.Text;

namespace FurnitureStore.Client.Admin.Services
{
    public class StaffService : IStaffService
    {
        private readonly HttpClient _httpClient;

        public StaffService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StaffDTO> LoginStaffAsync(string email, string password)
        {
            string apiUrl = $"{GlobalConfig.STAFF_BASE_URL}/login";

            var json = JsonConvert.SerializeObject(new
            {
                email = email,
                password = EncryptData(password)
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(new Uri(apiUrl), content);

            if (response.IsSuccessStatusCode)
            {
                string? jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<StaffDTO>(jsonResponse!)!;
            }

            return null!;
        }

        private string EncryptData(string data)
        {
            try
            {
                byte[] encDataByte = Encoding.UTF8.GetBytes(data!);
                string encodeData = Convert.ToBase64String(encDataByte);
                return encodeData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode " + ex.Message);
            }
        }
    }
}
