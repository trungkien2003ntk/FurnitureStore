using Newtonsoft.Json;

namespace FurnitureStore.Server.Models.Documents
{
    public class OrderItem
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("sellingPrice")]
        public int SellingPrice { get; set; }

        [JsonProperty("totalPrice")]
        public int TotalPrice { get; set; }
    }

    public class ShippingInfo
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("shippingPrice")]
        public int ShippingPrice { get; set; }

        [JsonProperty("shippingDate")]
        public DateTime ShippingDate { get; set; }
    }


    public class OrderDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("customerType")]
        public string CustomerType { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("items")]
        public List<OrderItem> Items { get; set; }

        [JsonProperty("shippingInfo")]
        public ShippingInfo ShippingInfo { get; set; }

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

    
}
