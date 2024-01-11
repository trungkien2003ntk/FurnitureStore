using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Shared.Responses;

public class OrderResponse
{
    [JsonProperty("data")]
    public List<OrderDTO> Data { get; set; }

    [JsonProperty("metadata")]
    public Metadata Metadata { get; set; }
}
