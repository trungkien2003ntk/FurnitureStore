namespace FurnitureStore.Server.Models.BindingModels.FilterModels
{
    public class CategoryFilterModel
    {
        [FromQuery(Name = "useNestedResult")]
        public bool UseNestedResult { get; set; } = false;

        [FromQuery(Name ="level")]
        public int? Level { get; set; }

        [FromQuery(Name = "parentId")]
        public string? ParentId { get; set; }
    }
}
