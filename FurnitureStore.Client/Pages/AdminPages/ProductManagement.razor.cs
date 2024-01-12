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
        [Inject]
        ICategoryService categoryService { get; set; } = null!;
        public IEnumerable<ProductDTO> productList { get; set; } = new List<ProductDTO>();
        public IEnumerable<ProductDTO> productVariantList { get; set; } = new List<ProductDTO>();
        public IEnumerable<CategoryDTO> categoryListLV1 { get; set; } = new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> categoryListLV2 { get; set; } = new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> categoryListLV3 { get; set; } = new List<CategoryDTO>();
        private bool isHidden { get; set; } = true;
        private bool isHiddenVariant { get; set; } = true;
        private bool isChecked { get; set; } = false;
        private bool isClicked { get; set; } = false;
        private string SelectedCategoryLV1Text { get; set; } = "Choose Category";
        private string SelectedCategoryLV2Text { get; set; } = "Choose Category";
        private string SelectedCategoryLV3Text { get; set; } = "Choose Category";
        int productCount = 0;
        int pageSize = 15;
        int pageNumber = 0;
        int selectedPageNumber = 1;
        string currentCategoryId;
        List<int> pageNumberList = new List<int>();
        List<string> categoryIdList = new List<string>();

        private string ProductNameInput { get; set; } = "";
        private string ProductWeightInput { get; set; } = ""; 
        private string ProductStockInput { get; set; } = "";
        private int HeightInput { get; set; } = 0;
        private int WidthInput { get; set; } = 0;
        private int DepthInput { get; set; } = 0;
        private int PurchasePriceInput { get; set; } = 0;
        private int RegularPriceInput { get; set; } = 0;
        private int SalePriceInput { get; set; } = 0;
        private string ProductSescriptionInput { get; set; } = "";
        private string VariantNameInput { get; set; } = "";
        private string FeatureImg { get; set; } = ""; 

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

            //Get category dropdown
            var categoryLV1Respons = await categoryService.GetCategoryDTOsByLevelAsync(1);
            if (categoryLV1Respons != null)
            {
                categoryListLV1 = categoryLV1Respons.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV1 = new List<CategoryDTO>();
            }
        }

        private void HandlePageNumber(int pageCount, int pageSize)
        {
            pageNumberList.Clear();
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
            categoryIdList.Clear();
            if (currentCategoryId != null)
            {
                categoryIdList.Add(currentCategoryId);
            }
            if (categoryIdList.Count > 0)
            {
                var productResponse = await productService.GetProductDTOsAsync(categoryIdList, null, pageSize, pageNumber);
                if (productResponse != null)
                {
                    productList = productResponse.Data;
                }
            }
            else
            {
                var productResponse = await productService.GetProductDTOsAsync(null, null, pageSize, pageNumber);
                if (productResponse != null)
                {
                    productList = productResponse.Data;
                }
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
        #region Select category
        public async Task SelectLV1Category(string Id, string Text)
        {
            SelectedCategoryLV1Text = Text;
            currentCategoryId = Id;
            var categoryResponse = await categoryService.GetCategoryDTOsByParentIdAsync(Id);
            if (categoryResponse != null)
            {
                categoryListLV2 = categoryResponse.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV2 = new List<CategoryDTO>();
            }
            categoryListLV3 = new List<CategoryDTO>();
            List<string> currCategoryId = new List<string>();
            currCategoryId.Add(Id);
            var productResponse = await productService.GetProductDTOsAsync(currCategoryId, null, null, null);
            if(productResponse != null)
            {
                productList = productResponse.Data;
            }
            else
            {
                productList = new List<ProductDTO>();
            }
            productCount = productList.Count();
            HandlePageNumber(productCount, pageSize);
            await UpdatePagination();
        }

        public async Task SelectLV2Category(string Id, string Text)
        {
            SelectedCategoryLV2Text = Text;
            currentCategoryId = Id;
            var categoryResponse = await categoryService.GetCategoryDTOsByParentIdAsync(Id);
            if (categoryResponse != null)
            {
                categoryListLV3 = categoryResponse.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV3 = new List<CategoryDTO>();
            }
            List<string> currCategoryId = new List<string>();
            currCategoryId.Add(Id);
            var productResponse = await productService.GetProductDTOsAsync(currCategoryId, null, null, null);
            if (productResponse != null)
            {
                productList = productResponse.Data;
            }
            else
            {
                productList = new List<ProductDTO>();
            }
            productCount = productList.Count();
            HandlePageNumber(productCount, pageSize);
            await UpdatePagination();
        }
        public async Task SelectLV3Category(string Id, string Text)
        {
            SelectedCategoryLV3Text = Text;
            currentCategoryId = Id;
            List<string> currCategoryId = new List<string>();
            currCategoryId.Add(Id);
            var productResponse = await productService.GetProductDTOsAsync(currCategoryId, null, null, null);
            if (productResponse != null)
            {
                productList = productResponse.Data;
            }
            else
            {
                productList = new List<ProductDTO>();
            }
            productCount = productList.Count();
            HandlePageNumber(productCount, pageSize);
            await UpdatePagination();
        }

        private async Task SelectProduct(string Id)
        {
            ProductDTO currProduct = await productService.GetProductDTOByIdAsync(Id);
            if (currProduct != null && currProduct.VariationDetail.Id != null)
            {
                var productResponse = await productService.GetProductDTOsAsync(null, currProduct.VariationDetail.Id, null, null);
                if (productResponse != null)
                {
                    productVariantList = productResponse.Data;
                }
                else
                {
                    productVariantList = new List<ProductDTO>();
                }
                isClicked = true;
            }
        }
        #endregion

        #region Select category add update
        private string SelectedCategoryLV1TextAddUpdate { get; set; } = "Choose Category";
        private string SelectedCategoryLV2TextAddUpdate { get; set; } = "Choose Category";
        private string SelectedCategoryLV3TextAddUpdate { get; set; } = "Choose Category";
        public async Task SelectLV1CategoryAddUpdate(string Id, string Text)
        {
            SelectedCategoryLV1TextAddUpdate = Text;
            currentCategoryId = Id;
            var categoryResponse = await categoryService.GetCategoryDTOsByParentIdAsync(Id);
            if (categoryResponse != null)
            {
                categoryListLV2 = categoryResponse.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV2 = new List<CategoryDTO>();
            }
            categoryListLV3 = new List<CategoryDTO>();
        }

        public async Task SelectLV2CategoryAddUpdate(string Id, string Text)
        {
            SelectedCategoryLV2TextAddUpdate = Text;
            currentCategoryId = Id;
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
        public async Task SelectLV3CategoryAddUpdate(string Id, string Text)
        {
            SelectedCategoryLV3TextAddUpdate = Text;
            currentCategoryId = Id;
        }
        #endregion
        //add img button
        public void AddImgButton()
        {

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
