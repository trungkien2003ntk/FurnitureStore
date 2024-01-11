namespace FurnitureStore.Shared.DTOs;

public class OrderDTO
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("orderId")]
    public string? OrderId { get; set; }

    [JsonProperty("yearMonth")]
    public string? YearMonth { get; set; }

    [JsonProperty("customerType")]
    public string? CustomerType { get; set; } = "Retail";

    [JsonProperty("customerName")]
    public string CustomerName { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }

    [JsonProperty("items")]
    public List<OrderItem> Items { get; set; } = [];

    [JsonProperty("shippingInfo")]
    public OrderShippingInfo? ShippingInfo { get; set; }

    [JsonProperty("totalItems")]
    public int TotalItems { get; set; } = 0;

    [JsonProperty("subtotal")]
    public int Subtotal { get; set; } = 0;

    [JsonProperty("discountValue")]
    public int DiscountValue { get; set; } = 0;

    [JsonProperty("tax")]
    public int Tax { get; set; } = 0;

    [JsonProperty("totalAmount")]
    public int TotalAmount { get; set; } = 0;

    [JsonProperty("paymentDetails")]
    public PaymentDetails PaymentDetails { get; set; } = new();

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("note")]
    public string? Note { get; set; }

    [JsonProperty("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("modifiedAt")]
    public DateTime? ModifiedAt { get; set; }

    [JsonProperty("ttl")]
    public int? TTL { get; set; }
}


