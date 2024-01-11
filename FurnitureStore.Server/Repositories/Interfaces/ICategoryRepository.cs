﻿using FurnitureStore.Server.Models.Documents;

namespace FurnitureStore.Server.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task AddCategoryDocumentAsync(CategoryDocument item);
    Task UpdateCategoryAsync(CategoryDTO item);
    Task AddCategoryDTOAsync(CategoryDTO item);
    Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevelAsync(int level);
    Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParentAsync(string? parent);
    Task<IEnumerable<CategoryResponse>> GetCategoryResponsesAsync();
    Task<CategoryDTO> GetCategoryDTOByIdAsync(string id);
    Task<IEnumerable<CategoryDTO>> GetCategoryDTOsAsync();
    Task DeleteCategoryAsync(string id);
    Task<string> GetNewCategoryIdAsync();
    Task<IEnumerable<CategoryDetailDocument>> GetFullInformationOfAllCategories();
}
