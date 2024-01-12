using FurnitureStore.Client.Admin.IServices;
using FurnitureStore.Client.Admin.Services;
using FurnitureStore.Shared.DTOs;
using FurnitureStore.Shared.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace FurnitureStore.Client.Admin.Pages.AdminPages
{
    public partial class ProductManagement
    {
        [Inject]
        IProductService ProductService { get; set; } = null!;
        [Inject]
        ICategoryService CategoryService { get; set; } = null!;
        public IEnumerable<ProductDTO> ProductList { get; set; } = new List<ProductDTO>();
        public IEnumerable<ProductDTO> ProductVariantList { get; set; } = new List<ProductDTO>();
        public IEnumerable<CategoryDTO> CategoryListLV1 { get; set; } = new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> CategoryListLV2 { get; set; } = new List<CategoryDTO>();
        public IEnumerable<CategoryDTO> CategoryListLV3 { get; set; } = new List<CategoryDTO>();
        private bool IsHiddenProductModal { get; set; } = true;
        private bool IsHiddenVariant { get; set; } = true;
        private bool IsChecked { get; set; } = false;
        private bool IsClicked { get; set; } = false;
        private bool isAddNewMode = true;
        private bool IsAddNewMode
        {
            get => isAddNewMode; 
            set
            {
                isAddNewMode = value;

                if (isAddNewMode)
                {
                    CurrentSelectProduct = new();
                    IsHiddenVariant = true;
                }
            }
        } 
        private string SelectedCategoryLV1Text { get; set; } = "Choose Category";
        private string SelectedCategoryLV2Text { get; set; } = "Choose Category";
        private string SelectedCategoryLV3Text { get; set; } = "Choose Category";
        int productCount = 0;
        int pageSize = 15;
        int pageNumber = 0;
        int selectedPageNumber = 1;
        string? currentCategoryId;
        List<int> pageNumberList = [];
        List<string> categoryIdList = [];




        //variant management
        private ProductDTO lowestPriceProductVariant { get; set; }
        private int CurrNumberOfVariant { get; set; } = 2;
        private ProductDTO CurrentSelectProduct {  get; set; } = new();
        private List<ProductDTO> ProductVariants { get; set; } = [new(), new()];

        protected override async Task OnInitializedAsync()
        {
            //Get pagination
            var productResponse = await ProductService.GetProductResponseAsync(null, null, null, null);

            AssignValueToProductListAndCountField(productResponse);


            RecalculatePageNumbers(productCount, pageSize);
            pageNumber = 1;
            await UpdatePagination();

            //Get category dropdown
            var categoryLV1Response = await CategoryService.GetCategoryDTOsByLevelAsync(1);

            if (categoryLV1Response != null && categoryLV1Response.Any())
            {
                CategoryListLV1 = categoryLV1Response.Select(response => response.Category).ToList();
            }
            else
            {
                CategoryListLV1 = [];
            }
        }

        private void AddNewVariant()
        {
            if (IsAddNewMode)
            {
                CurrNumberOfVariant++;
                ProductVariants.Add(new ProductDTO { });
            }
        }

        private void ShowProductModalToAdd()
        {
            IsHiddenProductModal = false;
            IsAddNewMode = true;
        }

        private void ShowProductModalToUpdate()
        {
            IsHiddenProductModal = false;
            IsAddNewMode = false;
        }

        private void CloseProductModal()
        {
            IsHiddenProductModal = true;
        }
        private void CancelAddUpdatePopup()
        {
            IsHiddenProductModal = true;
        }
        private void VariantToggleCheckbox(ChangeEventArgs e)
        {
            IsChecked = (bool)e.Value;

            if (IsChecked)
            {
                IsHiddenVariant = false;
            }
            else
            {
                IsHiddenVariant = true;
            }
        }

        private void HandleProductChanged(ProductDTO product)
        {
            // Handle the ProductDTO here

        }

        private void SaveChanges()
        {
            if (!IsHiddenVariant)
            foreach (var productVariant in ProductVariants)
            {
                
            }
            else
            {

            }
        }

        private void Cancel()
        {
            IsHiddenProductModal = true;
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

        private void HandleVariantRemoved(int index)
        {
            ProductVariants.RemoveAt(index);
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
            
            var categoryLevel2Responses = await CategoryService.GetCategoryDTOsByParentIdAsync(Id);

            if (categoryLevel2Responses != null && categoryLevel2Responses.Any())
            {
                CategoryListLV2 = categoryLevel2Responses.Select(response => response.Category).ToList();
            }
            else
            {
                CategoryListLV2 = [];
            }

            Console.WriteLine($"Get category lv2 after clicked level 1, count: {CategoryListLV2.Count()}");

            var productResponse = await ProductService.GetProductResponseAsync(categoryIds: [Id], pageSize: pageSize, pageNumber: 1);

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

            var categoryResponses = await CategoryService.GetCategoryDTOsByParentIdAsync(Id);
            if (categoryResponses != null)
            {
                CategoryListLV3 = categoryResponses.Select(response => response.Category).ToList();
            }
            else
            {
                CategoryListLV3 = new List<CategoryDTO>();
            }

            var productResponse = await ProductService.GetProductResponseAsync([Id] );
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

            var productResponse = await ProductService.GetProductResponseAsync([Id], null, null, null);
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
                var productResponse = await ProductService.GetProductResponseAsync([currentCategoryId], null, pageSize, pageNumber);

                AssignValueToProductListAndCountField(productResponse);
            }
            else
            {
                var productResponse = await ProductService.GetProductResponseAsync(null, null, pageSize, pageNumber);
                AssignValueToProductListAndCountField(productResponse);
            }

            Console.WriteLine($"product list: {ProductList.Count()}");
        }

        private async Task SelectProduct(string Id)
        {
            CurrentSelectProduct = await ProductService.GetProductDTOByIdAsync(Id);

            ShowProductModalToUpdate();
        

            if (CurrentSelectProduct != null && CurrentSelectProduct.VariationDetail.Id != null)
            {
                var productResponse = await ProductService.GetProductResponseAsync(variationId: CurrentSelectProduct.VariationDetail.Id);
                if (productResponse != null)
                {
                    ProductVariants.Clear();
                    ProductVariants = new(productResponse.Data);
                    CurrNumberOfVariant = productResponse.Metadata.Count;

                    Console.WriteLine(productResponse.Metadata.Count);
                    IsHiddenVariant = false;
                }
                else
                {
                    ProductVariants = [];
                    IsHiddenVariant = true;
                }

                IsClicked = true;
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



        private void AssignValueToProductListAndCountField(ProductResponse? productResponse)
        {
            if (productResponse != null && productResponse.Data.Count > 0)
            {
                ProductList = productResponse.Data;
                productCount = productResponse.Metadata.Count;
            }
            else
            {
                ProductList = [];
                productCount = 0;
            }
        }

    }
}
