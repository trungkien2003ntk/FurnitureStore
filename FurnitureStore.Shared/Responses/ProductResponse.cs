using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Shared.Responses;

public class ProductResponse
{
    [JsonProperty("data")]
    public List<ProductDTO> Data { get; set; }

    [JsonProperty("metadata")]
    public Metadata Metadata { get; set; }
}
