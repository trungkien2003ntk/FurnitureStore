namespace FurnitureStore.Shared
{
    public class CartItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("categoryPath")]
        public string CategoryPath { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("regular_price")]
        public int RegularPrice { get; set; }

        [JsonProperty("sale_price")]
        public int SalePrice { get; set; }

        [JsonProperty("grams")]
        public int Grams { get; set; }

        [JsonProperty("featured_image")]
        public string FeaturedImage { get; set; }

        [JsonProperty("options")]
        public List<object> Options { get; set; }
    }

    public class CartDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("itemCount")]
        public int ItemCount { get; set; }

        [JsonProperty("items")]
        public List<CartItem> Items { get; set; }

        [JsonProperty("itemsSubtotalPrice")]
        public int ItemsSubtotalPrice { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("requiresShipping")]
        public bool RequiresShipping { get; set; }

        [JsonProperty("discount")]
        public int Discount { get; set; }

        [JsonProperty("tax")]
        public int Tax { get; set; }

        [JsonProperty("totalprice")]
        public int Totalprice { get; set; }

        [JsonProperty("totalWeight")]
        public int TotalWeight { get; set; }
    }
}
