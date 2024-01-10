namespace FunctionAppChangeFeed.Models;

public class Category
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

    [JsonProperty("isDeletable")]
    public bool IsDeletable { get; set; }

    [JsonProperty("isDeleted")]
    public bool IsDeleted { get; set; }

    [JsonProperty("ttl")]
    public int TTL { get; set; }
}
