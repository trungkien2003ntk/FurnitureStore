using FurnitureStore.Client.IServices;
using FurnitureStore.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class ProductManagement
    {
        [Inject]
        IProductService productService { get; set; } = null!;
        public IEnumerable<ProductDTO> productList { get; set; } = new List<ProductDTO>();
        private bool isHidden { get; set; } = true;
        private bool isHiddenVariant { get; set; } = true;
        private bool isChecked { get; set; } = false;


        protected override async Task OnInitializedAsync()
        {
            await GetProductList();
            await base.OnInitializedAsync();
        }

        public async Task GetProductList()
        {
            productList = await productService.GetAllProduct();
        }
        private void ShowAddUpdateProductPopup()
        {
            isHidden = false;
        }

        private void CloseAddUpdatePopup()
        {
            isHidden = true;
        }
        private void CancelAddUpdatePopup()
        {
            isHidden = true;
        }
        private void VariantToggleCheckbox(ChangeEventArgs e)
        {
            isChecked = (bool)e.Value;
            if (isChecked)
            {
                isHiddenVariant = false;
            }
            else
            {
                isHiddenVariant = true;
            }
        }

        
    }
}
