using FurnitureStore.Client.Admin.IServices;
using FurnitureStore.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using static System.Net.Mime.MediaTypeNames;

namespace FurnitureStore.Client.Admin.Pages.AdminPages
{
    public partial class CategoryManagement
    {
        #region Properties
        [Inject]
        ICategoryService categoryService { get; set; } = null!;
        public IEnumerable<CategoryDTO> categoryListLV1 { get; set; } =new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> categoryListLV2 { get; set; } = new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> categoryListLV3 { get; set; } = new List<CategoryDTO>();
        private string selectedCategoryId="";
        private string selectedCategoryId2 = "";
        private string selectedCategoryId3 = "";
        private string currentCategoryId = "";


        private bool isHiddenPopup { get; set; } = true; 
        private bool isHiddenNotification { get; set; } = true;
        private bool isDeleteNotification { get; set; } = true;
        private string AddUpdateCategoryForm { get; set; } = "";
        private string CategoryNameInput { get; set; } = "";
        private string LevelLabel { get; set; } = "";
        private string NotificationDetail { get; set; } = "";
        #endregion

        #region Initialized
        protected override async Task OnInitializedAsync()
        {
            await GetCategoryByLevel1();
                
            await base.OnInitializedAsync();
        }
        #endregion

        #region Button Click
        public void AddCategoryLV1()
        {
            CategoryNameInput = "";
            isHiddenPopup = false;
            AddUpdateCategoryForm = "Add category form";
            LevelLabel = "Level 1";
        }
        public void AddCategoryLV2()
        {
            if(selectedCategoryId != "")
            {
                CategoryNameInput = "";
                isHiddenPopup = false;
                AddUpdateCategoryForm = "Add category form";
                LevelLabel = "Level 2";
            }
            else { 
                isHiddenNotification = false;
                NotificationDetail = "You must choose category parent before add new childen";
            }
        }
        public void AddCategoryLV3()
        {
            if (selectedCategoryId2 != "")
            {
                CategoryNameInput = "";
                isHiddenPopup = false;
                AddUpdateCategoryForm = "Add category form";
                LevelLabel = "Level 3";
            }
            else
            {
                isHiddenNotification = false;
                NotificationDetail = "You must choose category parent before add new childen";
            }
        }
        public void CloseAddUpdateCategoryPopup()
        {
            isHiddenPopup = true;
        }
        public void CancelAddUpdateCategoryPopup()
        {
            isHiddenPopup = true;
        }

        public void CancelNotification()
        {
            isHiddenNotification = true;
        }

        public void CloseDeleteNotification()
        {
            isDeleteNotification = true;
        }

        public void NoButton()
        {
            isDeleteNotification = true;
        }
        private void DeleteSelectedCategory()
        {
            if(currentCategoryId != "")
            {
                isDeleteNotification = false;
            }
            else
            {
                isHiddenNotification = false;
                NotificationDetail = "You must choose category before delete";

            }
        }
        private async Task UpdateSelectedCategory()
        {
            if (currentCategoryId != "")
            {
                isHiddenPopup = false;
                AddUpdateCategoryForm = "Update category form";
                if (currentCategoryId == selectedCategoryId)
                {
                    LevelLabel = "Level 1";
                }
                else if (currentCategoryId == selectedCategoryId2)
                {
                    LevelLabel = "Level 2";
                }
                else if (currentCategoryId == selectedCategoryId3)
                {
                    LevelLabel = "Level 3";
                }
                CategoryDTO category = await categoryService.GetCategoryDTOsById(currentCategoryId);
                CategoryNameInput = category.Text ?? "";
            }
            else
            {
                isHiddenNotification = false;
                NotificationDetail = "You must choose category before update";

            }
        }

        #endregion

        #region OnSelected Category
        private async Task SelectCategory(string Id)
        {
            selectedCategoryId = Id;
            currentCategoryId = Id;
            selectedCategoryId2 = "";
            selectedCategoryId3 = "";
            var categoryResponse = await categoryService.GetCategoryDTOsByParentIdAsync(Id);
            if(categoryResponse != null)
            {
                categoryListLV2 = categoryResponse.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV2 = new List<CategoryDTO>();
            }
            categoryListLV3= new List<CategoryDTO>();
        }

        private async Task SelectCategory2(string Id)
        {
            selectedCategoryId2 = Id;
            currentCategoryId = Id;
            selectedCategoryId3 = "";
            var categoryResponse = await categoryService.GetCategoryDTOsByParentIdAsync(Id);
            if (categoryResponse != null)
            {
                categoryListLV3 = categoryResponse.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV3 = new List<CategoryDTO>();
            }
        }

        private void SelectCategory3(string Id)
        {
            selectedCategoryId3 = Id;
            currentCategoryId = Id;
        }
        #endregion

        #region Sort category
        private string SortNameCategoryLV1 { get; set; } = "Sort by";
        private string SortNameCategoryLV2 { get; set; } = "Sort by";
        private string SortNameCategoryLV3 { get; set; } = "Sort by";
        private void SortByIdCategoryLV1()
        {
            categoryListLV1=categoryListLV1.OrderBy(x => x.Id).ToList();
            SortNameCategoryLV1 = "ID";
        }
        private void SortByNameCategoryLV1()
        {
            categoryListLV1 = categoryListLV1.OrderBy(x => x.Text).ToList();
            SortNameCategoryLV1 = "Name";
        }
        private void SortByIdCategoryLV2()
        {
            categoryListLV2 = categoryListLV2.OrderBy(x => x.Id).ToList();
            SortNameCategoryLV2 = "ID";
        }
        private void SortByNameCategoryLV2()
        {
            categoryListLV2 = categoryListLV2.OrderBy(x => x.Text).ToList();
            SortNameCategoryLV2 = "Name";
        }
        private void SortByIdCategoryLV3()
        {
            categoryListLV3 = categoryListLV3.OrderBy(x => x.Id).ToList();
            SortNameCategoryLV3 = "ID";
        }
        private void SortByNameCategoryLV3()
        {
            categoryListLV3 = categoryListLV3.OrderBy(x => x.Text).ToList();
            SortNameCategoryLV3 = "Name";
        }

        #endregion

        #region Category Management
        

        private async Task YesButton()
        {
            if (currentCategoryId != "")
            {

                var categoryChildrenList = await categoryService.GetCategoryDTOsByParentIdAsync(currentCategoryId);
                if (categoryChildrenList == null)
                {
                    var category = await categoryService.GetCategoryDTOByIdAsync(currentCategoryId);
                    int level = category.Level;
                    var result = await categoryService.DeleteCategoryDTOAsync(currentCategoryId);
                    isDeleteNotification = true;
                    isHiddenNotification = false;
                    if (result)
                    {
                        if (level == 1)
                        {
                            await GetCategoryByLevel1();
                        }
                        else if (level == 2)
                        {
                            await GetCategoryLV2ByParentId(currentCategoryId);
                        }
                        else if (level == 3)
                        {
                            await GetCategoryLV3ByParentId(currentCategoryId);
                        }
                        NotificationDetail = "Delete category successfully";

                    }
                    else
                    {
                        NotificationDetail = "Error to delete category";
                    }
                }
                else
                {
                    isDeleteNotification = true;
                    isHiddenNotification = false;
                    NotificationDetail = "You must delete children category before delete this";
                }
            }
        }

        public async Task SaveChangeButton()
        {
            if(CategoryNameInput != "")
            {
                if (AddUpdateCategoryForm == "Add category form")
                {
                    CategoryDTO category = new CategoryDTO
                    {
                        Text = CategoryNameInput,
                        Level = 0
                    };
                    if (LevelLabel == "Level 1")
                    {
                        category.ParentId = "";
                        category.Level = 1;
                    }
                    else if (LevelLabel == "Level 2")
                    {
                        category.ParentId = selectedCategoryId;
                        category.Level = 2;
                    }
                    else if (LevelLabel == "Level 3")
                    {
                        category.ParentId = selectedCategoryId2;
                        category.Level = 3;
                    }
                    var result = await categoryService.AddCategoryAsync(category);
                    if (result !=null)
                    {
                        if (category.Level == 1)
                        {
                            await GetCategoryByLevel1();
                        }
                        else if (category.Level == 2)
                        {
                            await GetCategoryLV2ByParentId(currentCategoryId);
                        }
                        else if (category.Level == 3)
                        {
                            await GetCategoryLV3ByParentId(currentCategoryId);
                        }
                        isHiddenPopup = true;
                    }
                }
                else if(AddUpdateCategoryForm == "Update category form")
                {
                    CategoryDTO category = await categoryService.GetCategoryDTOsById(currentCategoryId) ?? new CategoryDTO();
                    category.Text= CategoryNameInput;
                    var result = await categoryService.UpdateCategoryDTOAsync(category.CategoryId!, category);
                    if (result)
                    {
                        if (category.Level == 1)
                        {
                            await GetCategoryByLevel1();
                        }
                        else if (category.Level == 2)
                        {
                            await GetCategoryLV2ByParentId(currentCategoryId);
                        }
                        else if(category.Level == 3)
                        {
                            await GetCategoryLV3ByParentId(currentCategoryId);
                        }
                        isHiddenPopup = true;
                    }
                }
            }
        }
        #endregion

        #region Helper
        public async Task GetCategoryByLevel1()
        {
            var categoryResponse = await categoryService.GetCategoryDTOsByLevelAsync(1);
            if (categoryResponse != null)
            {
                categoryListLV1 = categoryResponse.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV1=new List<CategoryDTO>();
            }
        }

        public async Task GetCategoryLV2ByParentId(string parentId)
        {
            var categoryResponse = await categoryService.GetCategoryDTOsByParentIdAsync(parentId);
            if (categoryResponse != null)
            {
                categoryListLV2 = categoryResponse.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV2 = new List<CategoryDTO>();
            }
        }
        public async Task GetCategoryLV3ByParentId(string parentId)
        {
            var categoryResponse = await categoryService.GetCategoryDTOsByParentIdAsync(parentId);
            if (categoryResponse != null)
            {
                categoryListLV3 = categoryResponse.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV3 = new List<CategoryDTO>();
            }
        }
        #endregion
    }
}
