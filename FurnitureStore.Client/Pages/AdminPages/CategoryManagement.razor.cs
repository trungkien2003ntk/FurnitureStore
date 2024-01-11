using FurnitureStore.Client.IServices;
using FurnitureStore.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class CategoryManagement
    {
        [Inject]
        ICategoryService categoryService { get; set; } = null!;
        public IEnumerable<CategoryDTO> categoryListLV1 { get; set; } =new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> categoryListLV2 { get; set; } = new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> categoryListLV3 { get; set; } = new List<CategoryDTO>();
        private string selectedCategoryId="";
        private string selectedCategoryPath="";
        private string selectedCategoryId2 = "";
        private string selectedCategoryPath2 = "";
        private string selectedCategoryId3 = "";
        private string currentCategoryId = "";


        private bool isHiddenPopup { get; set; } = true; 
        private bool isHiddenNotification { get; set; } = true;
        private string AddUpdateCategoryForm { get; set; } = "";
        private string CategoryNameInput { get; set; } = "";
        private string LevelLabel { get; set; } = "";

        protected override async Task OnInitializedAsync()
        {
            await GetCategoryByLevel1();
                
            await base.OnInitializedAsync();
        }

        public async Task GetCategoryByLevel1()
        {
            categoryListLV1 = await categoryService.GetCategoryDTOsByLevel(1);
        }

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
            else { isHiddenNotification = false; }
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
            else { isHiddenNotification = false; }
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
        //Select a category item
        private async Task SelectCategory(string Id, string path)
        {
            selectedCategoryId = Id;
            currentCategoryId = Id;
            selectedCategoryPath = path;
            selectedCategoryId2 = "";
            selectedCategoryId3 = "";
            path = path.Remove(0, 1);
            categoryListLV2 = await categoryService.GetCategoryDTOsByParent(path) ?? new List<CategoryDTO>();
        }

        private async Task SelectCategory2(string Id, string path)
        {
            selectedCategoryId2 = Id;
            currentCategoryId = Id;
            selectedCategoryPath2 = path;
            selectedCategoryId3 = "";
        }

        private async Task SelectCategory3(string Id, string path)
        {
            //do sth
        }

        //Update category
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
            }
        }

        //Delete category
        private void DeleteSelectedCategory()
        {

        }


        //Save change
        public async Task SaveChangeButton()
        {
            if(CategoryNameInput != "")
            {
                CategoryDTO category = new CategoryDTO
                {
                    Name = CategoryNameInput,
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
                if (AddUpdateCategoryForm == "Add category form")
                {
                    isHiddenPopup = true;
                }
            }
        }
    }
}
