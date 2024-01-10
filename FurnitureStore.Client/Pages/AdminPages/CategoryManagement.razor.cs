using FurnitureStore.Client.IServices;
using FurnitureStore.Shared;
using Microsoft.AspNetCore.Components;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class CategoryManagement
    {
        [Inject]
        ICategoryService categoryService { get; set; } = null!;
        public IEnumerable<CategoryDTO> categorieListLV1 { get; set; } =new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> categorieListLV2 { get; set; } = new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> categorieListLV3 { get; set; } = new List<CategoryDTO>();
        private string selectedCategoryId;


        private bool isHiddenPopup { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await GetCategoryByLevel1();
                
            await base.OnInitializedAsync();
        }

        public async Task GetCategoryByLevel1()
        {
            categorieListLV1 = await categoryService.GetCategoryByLevel(1);
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
        private void SelectCategory(string Id)
        {
            selectedCategoryId = Id;

            Console.WriteLine($"Selected Category ID: {selectedCategoryId}");
        }
    }
}
