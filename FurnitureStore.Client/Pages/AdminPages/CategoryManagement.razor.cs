using FurnitureStore.Client.IServices;
using FurnitureStore.Shared;
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
        private string selectedCategoryName="";


        private bool isHiddenPopup { get; set; } = true;

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
            isHiddenPopup = false;
        }
        public void AddCategoryLV2()
        {
            isHiddenPopup = false;
        }
        public void AddCategoryLV3()
        {
            isHiddenPopup = false;
        }
        public void CloseAddUpdateCategoryPopup()
        {
            isHiddenPopup = true;
        }
        public void CancelAddUpdateCategoryPopup()
        {
            isHiddenPopup = true;
        }
        //Select a category item
        private async Task SelectCategory(string Id, string name)
        {
            selectedCategoryId = Id;
            selectedCategoryName = name;
            categoryListLV2 = await categoryService.GetCategoryDTOsByParent(name);
        }
    }
}
