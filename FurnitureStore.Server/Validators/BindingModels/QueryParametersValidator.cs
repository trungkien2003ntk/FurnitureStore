using FurnitureStore.Server.Models.BindingModels;

namespace FurnitureStore.Server.Validators.BindingModels
{
    public class QueryParametersValidator : AbstractValidator<QueryParameters>
    {
        public QueryParametersValidator()
        {
            List<string> validOrders = new() { "asc", "desc" };

            RuleFor(x => x.PageNumber)
                .NotEmpty()
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .NotEmpty()
                .GreaterThanOrEqualTo(-1);

            RuleFor(x => x.SortBy)
                .NotEmpty().When(x => !string.IsNullOrEmpty(x.OrderBy))
                .WithMessage("SortBy must be provided when OrderBy is provided.");

            RuleFor(x => x.OrderBy)
                .NotEmpty().When(x => !string.IsNullOrEmpty(x.SortBy))
                .WithMessage("OrderBy must be provided when SortBy is provided.")
                .Must(validOrders.Contains)
                .When(x => !string.IsNullOrEmpty(x.OrderBy))
                .WithMessage($"The Order property must be in \"{string.Join(", ", validOrders)}\".");
            
        }
    }
}
