using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Shared.Responses;

public class CategoryResponse
{
    public CategoryDTO Category { get; set; }
    public List<CategoryResponse>? SubCategories { get; set; }
    public int CountSub { get => SubCategories == null ? 0 : SubCategories.Count; }
}
