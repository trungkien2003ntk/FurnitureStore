using FurnitureStore.Client.IServices;
using FurnitureStore.Client.Services;
using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;
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
        string? currentCategoryId;
        List<int> pageNumberList = new List<int>();
        List<string> categoryIdList = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            //Get pagination
            var productResponse = await productService.GetProductResponseAsync(null, null, null, null);

            AssignValueToProductListAndCountField(productResponse);


            RecalculatePageNumbers(productCount, pageSize);
            pageNumber = 1;
            await UpdatePagination();

            //Get category dropdown
            var categoryLV1Response = await categoryService.GetCategoryDTOsByLevelAsync(1);

            if (categoryLV1Response != null && categoryLV1Response.Any())
            {
                categoryListLV1 = categoryLV1Response.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV1 = [];
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
            if (number != selectedPageNumber)
            {
                selectedPageNumber = number;
                pageNumber = number;
                await UpdatePagination();
            }
        }

        private async Task PreviousPage()
        {
            Console.WriteLine(string.Join(',', pageNumberList));

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
            if (pageNumberList == null || !pageNumberList.Any())
            {
                Console.WriteLine("Page number list is null or empty.");
                return;
            }

            Console.WriteLine("page number list: ", string.Join(',', pageNumberList));

            if (selectedPageNumber < pageNumberList.Max())
            {
                Console.WriteLine("selected: {0}", selectedPageNumber);

                selectedPageNumber++;
                pageNumber = selectedPageNumber;

                Console.WriteLine($"page number updated to {pageNumber}, now querying the database to update product list");

                await UpdatePagination();

                if (pageNumberList.Count > 4 && selectedPageNumber > pageNumberList[4])
                {
                    pageNumberList.RemoveAt(0);
                }
            }
        }
        #region Select category
        public async Task SelectLV1Category(string Id, string Text)
        {
            Console.WriteLine($"Clicked category lv1, id: {Id}");
            SelectedCategoryLV1Text = Text;

            currentCategoryId = Id;
            
            var categoryLevel2Responses = await categoryService.GetCategoryDTOsByParentIdAsync(Id);

            if (categoryLevel2Responses != null && categoryLevel2Responses.Any())
            {
                categoryListLV2 = categoryLevel2Responses.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV2 = [];
            }

            Console.WriteLine($"Get category lv2 after clicked level 1, count: {categoryListLV2.Count()}");

            var productResponse = await productService.GetProductResponseAsync(categoryIds: [Id], pageSize: pageSize, pageNumber: 1);

            AssignValueToProductListAndCountField(productResponse);

            RecalculatePageNumbers(productCount, pageSize);
            // reset page number to 1
            await SelectPageNumber(1);
        }

        public async Task SelectLV2Category(string Id, string Text)
        {
            Console.WriteLine($"Clicked category lv2, id: {Id}");

            SelectedCategoryLV2Text = Text;

            currentCategoryId = Id;

            var categoryResponses = await categoryService.GetCategoryDTOsByParentIdAsync(Id);
            if (categoryResponses != null)
            {
                categoryListLV3 = categoryResponses.Select(response => response.Category).ToList();
            }
            else
            {
                categoryListLV3 = new List<CategoryDTO>();
            }

            var productResponse = await productService.GetProductResponseAsync([Id] );
            AssignValueToProductListAndCountField(productResponse);

            RecalculatePageNumbers(productCount, pageSize);
            // reset page number to 1
            await SelectPageNumber(1);
        }
        public async Task SelectLV3Category(string Id, string Text)
        {
            Console.WriteLine($"Clicked category lv3, id: {Id}");

            SelectedCategoryLV3Text = Text;

            currentCategoryId = Id;

            var productResponse = await productService.GetProductResponseAsync([Id], null, null, null);
            AssignValueToProductListAndCountField(productResponse);

            RecalculatePageNumbers(productCount, pageSize);
            // reset page number to 1
            await SelectPageNumber(1);
        }

        private void RecalculatePageNumbers(int pageCount, int pageSize)
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
                for (int i = 0; i < pageNumber; i++)
                {
                    pageNumberList.Add(i + 1);
                }
            }
        }

        private async Task UpdatePagination()
        {
            Console.WriteLine($"currentcateryId: {currentCategoryId}");

            if (!string.IsNullOrEmpty(currentCategoryId))
            {
                var productResponse = await productService.GetProductResponseAsync([currentCategoryId], null, pageSize, pageNumber);

                AssignValueToProductListAndCountField(productResponse);
            }
            else
            {
                var productResponse = await productService.GetProductResponseAsync(null, null, pageSize, pageNumber);
                AssignValueToProductListAndCountField(productResponse);
            }

            Console.WriteLine($"product list: {productList.Count()}");
        }

        private async Task SelectProduct(string Id)
        {
            ProductDTO currProduct = await productService.GetProductDTOByIdAsync(Id);

            if (currProduct != null && currProduct.VariationDetail.Id != null)
            {
                var productResponse = await productService.GetProductResponseAsync(variationId: currProduct.VariationDetail.Id);
                if (productResponse != null)
                {
                    productVariantList = productResponse.Data;
                }
                else
                {
                    productVariantList = [];
                }
                isClicked = true;
            }
        }
        #endregion


        //update product button
        public void UpdateProduct(string Id)
        {

        }

        //delete product button
        public void DeleteProduct(string Id)
        {

        }



        private void AssignValueToProductListAndCountField(ProductResponse productResponse)
        {
            if (productResponse != null && productResponse.Data.Count > 0)
            {
                productList = productResponse.Data;
                productCount = productResponse.Metadata.Count;
            }
            else
            {
                productList = [];
                productCount = 0;
            }
        }

    }
}
