namespace FurnitureStore.Shared;

public class OrderDTO
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("orderId")]
    public string OrderId { get; set; }

    [JsonProperty("orderDate")]
    public DateTime OrderDate { get; set; }

    [JsonProperty("customerType")]
    public string CustomerType { get; set; }

    [JsonProperty("items")]
    public List<OrderItem> Items { get; set; }

    [JsonProperty("shippingInfo")]
    public OrderShippingInfo ShippingInfo { get; set; }

    [JsonProperty("totalItems")]
    public int TotalItems { get; set; }

    [JsonProperty("subtotal")]
    public int Subtotal { get; set; }

    [JsonProperty("discounts")]
    public int Discounts { get; set; }

    [JsonProperty("tax")]
    public int Tax { get; set; }

    [JsonProperty("totalAmount")]
    public int TotalAmount { get; set; }

    [JsonProperty("paymentMethod")]
    public string PaymentMethod { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("note")]
    public string Note { get; set; }

    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
}


