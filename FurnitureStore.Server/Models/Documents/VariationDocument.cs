namespace FurnitureStore.Server.Models.Documents
{
    public class VariationDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("variationId")]
        public string VariationId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("modifiedAt")]
        public DateTime? ModifiedAt { get; set; }

        [JsonProperty("ttl")]
        public int Ttl { get; set; }
    }
}
