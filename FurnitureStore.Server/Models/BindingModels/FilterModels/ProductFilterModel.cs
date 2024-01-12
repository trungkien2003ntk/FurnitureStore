using FurnitureStore.Server.Utils;

namespace FurnitureStore.Server.Models.BindingModels.FilterModels
{
    public class ProductFilterModel
    {

        private List<(int MinPrice, int MaxPrice)> _priceRanges;
        internal List<(int MinPrice, int MaxPrice)>? PriceRanges { get => _priceRanges; private set { } }


        [FromQuery(Name ="query")]
        public string? Query { get; set; }

        [FromQuery(Name ="variationId")]
        public string? VariationId { get; set; }

        [FromQuery(Name = "categoryIds")]
        [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder<string>))]
        public IEnumerable<string>? CategoryIds { get; set; }

        [FromQuery(Name = "productIds")]
        [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder<string>))]
        public IEnumerable<string>? ProductIds { get; set; }

        private IEnumerable<string>? _priceRangeStrings;
        [FromQuery(Name = "priceRanges")]
        [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder<string>))]
        public IEnumerable<string>? PriceRangeStrings
        {
            get => _priceRangeStrings;
            set
            {
                _priceRangeStrings = value;
                if (_priceRangeStrings == null || !_priceRangeStrings.Any()) { return; }

                _priceRanges = new();

                foreach (var priceRangeString in _priceRangeStrings)
                {
                    var parts = priceRangeString.Split('-');
                    int min = int.Parse(parts[0]);
                    int max = int.Parse(parts[1]);
                    _priceRanges.Add((min, max));
                }

            }
        }

        [FromQuery(Name ="isActive")]
        public bool? IsActive { get; set; }
    }
}
