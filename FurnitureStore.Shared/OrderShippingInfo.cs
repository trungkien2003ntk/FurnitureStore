namespace FurnitureStore.Shared;

public class OrderShippingInfo
{
    [JsonProperty("address")]
    public string Address { get; set; }

    [JsonProperty("shippingPrice")]
    public int ShippingPrice { get; set; }

    [JsonProperty("shippingDate")]
    public DateTime ShippingDate { get; set; }
}
