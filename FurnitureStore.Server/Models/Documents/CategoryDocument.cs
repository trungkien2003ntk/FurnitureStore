using Newtonsoft.Json;

namespace FurnitureStore.Server.Models.Documents
{
    public class CategoryDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("children")]
        public List<string> Children { get; set; }

        [JsonProperty("parent")]
        public string Parent { get; set; }

        [JsonProperty("categoryPath")]
        public string CategoryPath { get; set; }
    }
}
