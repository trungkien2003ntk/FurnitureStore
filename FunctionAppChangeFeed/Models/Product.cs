namespace FunctionAppChangeFeed.Models;

public class Product
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

    [JsonProperty("discountedPrice")]
    public int DiscountedPrice { get; set; }

    [JsonProperty("totalDiscount")]
    public int TotalDiscount { get; set; }

    [JsonProperty("discounts")]
    public List<object> Discounts { get; set; }

    [JsonProperty("grams")]
    public int Grams { get; set; }

    [JsonProperty("featuredImage")]
    public string FeaturedImage { get; set; }

    [JsonProperty("images")]
    public List<string> Images { get; set; }

    [JsonProperty("tags")]
    public List<string> Tags { get; set; }

    [JsonProperty("options")]
    public List<ProductOption> Options { get; set; }

    [JsonProperty("isDeleted")]
    public bool IsDeleted { get; set; }

    [JsonProperty("ttl")]
    public int TTL { get; set; }
}
