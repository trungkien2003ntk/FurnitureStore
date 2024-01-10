using FurnitureStore.Client.IServices;
using FurnitureStore.Shared;
using Microsoft.AspNetCore.Components;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class CategoryManagement
    {
        [Inject]
        IProductService productService { get; set; } = null!;
        public IEnumerable<ProductDTO> productList { get; set; } =new List<ProductDTO>();

        private bool isHiddenPopup { get; set; } = true;
        private string labelContent = "";

        protected override async Task OnInitializedAsync()
        {
            await GetProductList();
            foreach (var item in productList)
            {
                labelContent = item.Name;
            }
            await base.OnInitializedAsync();
        }

        public async Task GetProductList()
        {
            productList = await productService.GetAllProduct();
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
    }
}
