namespace FurnitureStore.Shared.Additional;

public class Dimensions
{
    [JsonProperty("height")]
    public int Height { get; set; } = 0;

    [JsonProperty("width")]
    public int Width { get; set; } = 0;

    [JsonProperty("depth")]
    public int Depth { get; set; } = 0;
}
