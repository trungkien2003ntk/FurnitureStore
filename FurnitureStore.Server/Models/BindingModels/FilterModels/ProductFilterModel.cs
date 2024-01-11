namespace FurnitureStore.Server.Models.BindingModels.FilterModels
{
    public class ProductFilterModel
    {
        [FromQuery(Name ="variationId")]
        public string? VariationId { get; set; }

        [FromQuery(Name = "categoryIds")]
        [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder<string>))]
        public IEnumerable<string>? CategoryIds { get; set; }

        [FromQuery(Name = "productIds")]
        [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder<string>))]
        public IEnumerable<string>? ProductIds { get; set; }
    }
}
