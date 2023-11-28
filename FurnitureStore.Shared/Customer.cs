namespace FurnitureStore.Shared
{
    public class PurchaseHistory
    {
        [JsonProperty("orderID")]
        public string? OrderID { get; set; }

        [JsonProperty("date")]
        public DateTime CreateAt { get; set; }

        [JsonProperty("totalAmount")]
        public int? TotalAmount { get; set; }
    }

    public class Customer
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("customerID")]
        public string? CustomerID { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("phone")]
        public string? Phone { get; set; }

        [JsonProperty("totalPay")]
        public int TotalPay { get; set; }

        [JsonProperty("purchaseHistory")]
        public List<PurchaseHistory>? PurchaseHistory { get; set; }
    }
}
