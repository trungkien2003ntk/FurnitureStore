using FurnitureStore.Client.IServices;
using FurnitureStore.Client.Services;
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
        int productCount = 0;
        int pageSize = 15;
        int pageNumber = 0;
        int selectedPageNumber = 1;
        List<int> pageNumberList = new List<int>();
        List<string> categoryIdList = null;


        protected override async Task OnInitializedAsync()
        {
            //Get pagination
            var productResponse = await productService.GetProductDTOsAsync(null, null, null, null);
            if (productResponse != null)
            {
                productList = productResponse.Data;
                productCount = productList.Count();
            }
            if (productCount > 0)
            {
                pageNumber = productCount / pageSize;
            }
            HandlePageNumber(productCount, pageSize);
            pageNumber = 1;
            await UpdatePagination();
        }

        private void HandlePageNumber(int pageCount, int pageSize)
        {
            if (pageCount > 0)
            {
                pageNumber = (pageCount / pageSize);
                int remain = pageCount % pageSize;
                if (remain > 0)
                {
                    pageNumber += 1;
                }
                for (int i = 0; i<pageNumber; i++)
                {
                    pageNumberList.Add(i + 1);
                }
            }
        }

        private async Task UpdatePagination()
        {
            var productResponse = await productService.GetProductDTOsAsync(categoryIdList,null, pageSize, pageNumber);
            if (productResponse != null)
            {
                productList = productResponse.Data;
            }
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

        private async Task SelectPageNumber(int number)
        {
            selectedPageNumber = number;
            pageNumber = number;
            await UpdatePagination();
        }
        private async Task PreviousPage()
        {
            if (selectedPageNumber > 1)
            {
                selectedPageNumber --;
                pageNumber = selectedPageNumber;
                await UpdatePagination();
                if (selectedPageNumber == pageNumberList[0]-1)
                {
                    pageNumberList.Add(pageNumberList.Max() + 1);
                    for(int i=0;i < pageNumberList.Count;i++)
                    {
                        pageNumberList[i]--;
                    }
                }
            }
        }
        private async Task NextPage()
        {
            if(selectedPageNumber < pageNumberList.Max())
            {
                selectedPageNumber ++;
                pageNumber = selectedPageNumber;
                await UpdatePagination();
                if (selectedPageNumber > pageNumberList[4])
                {
                    pageNumberList.RemoveAt(0);
                }
            }
            
        }
        //update product button
        public void UpdateProduct(string Id)
        {

        }

        //delete product button
        public void DeleteProduct(string Id)
        {

        }


    }
}
