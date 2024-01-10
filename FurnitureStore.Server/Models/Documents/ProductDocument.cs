using FurnitureStore.Shared.Additional;

namespace FurnitureStore.Server.Models.Documents
{
    public class ProductDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("categoryId")]
        public string CategoryId { get; set; }

        [JsonProperty("categoryPath")]
        public string CategoryPath { get; set; }

        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("stock")]
        public int Stock { get; set; }

        [JsonProperty("minStock")]
        public int MinStock { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("regularPrice")]
        public int RegularPrice { get; set; }

        [JsonProperty("salePrice")]
        public int SalePrice { get; set; }

        [JsonProperty("purchasePrice")]
        public int PurchasePrice { get; set; }

        [JsonProperty("variationDetail")]
        public VariationDetail VariationDetail { get; set; } = new VariationDetail();

        [JsonProperty("grams")]
        public int Grams { get; set; }

        [JsonProperty("dimensions")]
        public Dimensions Dimensions { get; set; } = new Dimensions();

        [JsonProperty("featuredImage")]
        public string FeaturedImage { get; set; }

        [JsonProperty("imagesUrl")]
        public List<string> ImagesUrl { get; set; } = [];

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = [];

        [JsonProperty("optionalDetails")]
        public List<ProductOption> OptionalDetails { get; set; } = [];

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("isRemovable")]
        public bool IsRemovable { get; set; }

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("modifiedAt")]
        public DateTime? ModifiedAt { get; set; }

        [JsonProperty("ttl")]
        public int TTL { get; set; } = -1;
    }
}
