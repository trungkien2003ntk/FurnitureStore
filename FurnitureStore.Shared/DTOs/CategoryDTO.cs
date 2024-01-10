namespace FurnitureStore.Shared.DTOs;

public class CategoryDTO
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("categoryId")]
    public string? CategoryId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("children")]
    public List<string>? Children { get; set; }

    [JsonProperty("parentId")]
    public string ParentId { get; set; }

    [JsonProperty("parentPath")]
    public string? ParentPath { get; set; }

    [JsonProperty("categoryPath")]
    public string? CategoryPath { get; set; }

    [JsonProperty("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }

    [JsonProperty("isRemovable")]
    public bool? IsRemovable { get; set; }

    [JsonProperty("isDeleted")]
    public bool? IsDeleted { get; set; }

    [JsonProperty("ttl")]
    public int? TTL { get; set; }
}
