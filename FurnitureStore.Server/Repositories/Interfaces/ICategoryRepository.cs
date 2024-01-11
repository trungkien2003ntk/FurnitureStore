﻿using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.BindingModels;

namespace FurnitureStore.Server.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryResponse>> GetCategoryResponsesAsync(QueryParameters queryParameters, CategoryFilterModel filter);
    Task<CategoryDTO> GetCategoryDTOByIdAsync(string id);
    Task<CategoryDTO?> AddCategoryDTOAsync(CategoryDTO categoryDTO);
    Task UpdateCategoryAsync(CategoryDTO item);
    Task DeleteCategoryAsync(string id);
}
