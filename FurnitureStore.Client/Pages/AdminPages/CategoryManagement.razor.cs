using FurnitureStore.Client.IServices;
using FurnitureStore.Shared;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class CategoryManagement
    {
        private readonly IServiceProvider _serviceProvider;

        private bool isHiddenPopup { get; set; } = true;
        private List<ProductDTO> productList = new List<ProductDTO>();
        private string labelContent = "";

        protected override async Task OnInitializedAsync()
        {
            GetProductList();
            foreach (var item in productList)
            {
                labelContent = item.CategoryId.ToString();
            }
            await base.OnInitializedAsync();
        }

        public async Task GetProductList()
        {
            var _productServer = _serviceProvider.GetService<IProductService>();
            productList = await _productServer.GetAllProduct();
        }
        public async void AddCategoryLV1()
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
