namespace FurnitureStore.Server.Models.BindingModels.FilterModels
{
    public class OrderFilterModel
    {
        [FromQuery(Name = "status")]
        public string? Status { get; set; }
    }
}
