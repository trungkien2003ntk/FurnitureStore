namespace FurnitureStore.Shared.Additional;

public class PaymentDetails
{
    [JsonProperty("remainAmount")]
    public double RemainAmount { get; set; } = 0;

    [JsonProperty("paidAmount")]
    public double PaidAmount { get; set; } = 0;

    [JsonProperty("paymentMethod")]
    public string? PaymentMethod { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }
}
