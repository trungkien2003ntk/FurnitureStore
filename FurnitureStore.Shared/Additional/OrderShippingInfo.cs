namespace FurnitureStore.Shared.Additional;

public class OrderShippingInfo
{
    [JsonProperty("address")]
    public string Address { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("shippingPrice")]
    public double ShippingPrice { get; set; }

    [JsonProperty("shippingDate")]
    public DateTime ShippingDate { get; set; }
}
