using Newtonsoft.Json;

namespace FurnitureStore.Server.Models.Documents
{
    internal class Staff
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("staffId")]
        public string StaffId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("passwordTemp")]
        public string PasswordTemp { get; set; }

        [JsonProperty("hashedAndSaltedPassword")]
        public string HashedAndSaltedPassword { get; set; }
    }
}
