namespace FurnitureStore.Shared.Responses;

public class ImageResponse
{
    [JsonProperty("data")]
    public List<string> Data { get; set; }

    [JsonProperty("metadata")]
    public Metadata Metadata {  get; set; }
}
