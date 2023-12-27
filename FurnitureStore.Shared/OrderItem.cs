namespace FurnitureStore.Shared
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
}
