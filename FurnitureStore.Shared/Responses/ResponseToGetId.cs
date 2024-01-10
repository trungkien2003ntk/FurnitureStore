namespace FurnitureStore.Shared.Responses;

public class ResponseToGetId
{
    [JsonProperty("id")]
    public string Id { get; set; }
}
