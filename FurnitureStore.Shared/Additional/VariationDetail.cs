namespace FurnitureStore.Shared.Additional;

public class VariationDetail
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("value")]
    public string? Value { get; set; }

    [JsonProperty("imageUrl")]
    public string? ImageUrl { get; set; }
}
