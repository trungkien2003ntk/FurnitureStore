namespace FurnitureStore.Server.Models.BindingModels.FilterModels
{
    public class ProductFilterModel
    {
        [FromQuery(Name ="variationId")]
        public string? VariationId { get; set; }
    }
}
