namespace FurnitureStore.Shared.Additional;

public class ProductOption
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }
}
