﻿using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Client.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevel(int level);
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParent(string parent);
        Task<CategoryDTO> GetCategoryDTOsById(string id);
        Task<CategoryDTO> AddCategory(CategoryDTO category);
        Task<bool> DeleteCategoryDTOAsync(string categoryId);
        Task<bool> UpdateCategoryDTOAsync(string categoryId, CategoryDTO categoryDTO);
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevelAsync(int level);
        Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByParentIdAsync(string parentId);
    }
}
