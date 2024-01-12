using FurnitureStore.Client.IServices;
using FurnitureStore.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class OrderManagement
    {
        [Inject]
        IOrderService orderService { get; set; } = null!;
        [Inject]
        IProductService productService { get; set; } = null!;
        List<OrderDTO> orderList { get; set; } = new List<OrderDTO>();
        List<ProductDTO> productList { get; set; } = new List<ProductDTO>();
        OrderDTO currentOrder { get; set; } = new OrderDTO();
        bool isHiddenNotification { get; set; } = true;
        string OrderStatus { get; set; } = "";
        string SortByText { get; set; } = "Sort by";
        string NotificationDetail { get; set; } = "";

        #region Hidden Show Popup
        private bool isHiddenPopup { get; set; } = true;

        public void CloseOrderDetailPopup()
        {
            isHiddenPopup = true;
        }

        public void CancelOrderDetailPopup()
        {
            isHiddenPopup = true;
        }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            
            var orderResponse = await orderService.GetOrderResponseAsync();
            if (orderResponse != null)
            {
                orderList = orderResponse.Data;
            }
            var productResponse = await productService.GetProductDTOsAsync();
            if (productResponse != null)
            {
                productList = productResponse.Data;
            }
            base.OnInitialized();
        }

        #region SelectOrder
        private async Task SelectOrder(string Id)
        {
            currentOrder = await orderService.GetOrderByIdAsync(Id) ?? new OrderDTO();
            isHiddenPopup = false;
        }
        #endregion

        private ProductDTO GetCurrentProduct(string Id)
        {
           foreach(var item in productList)
            {
                if (item.Id == Id)
                {
                    return item!;
                }
            }
            return null!;
        }

        private async Task SaveChanged()
        {
            if (currentOrder.Id != null)
            {
                var result = await orderService.UpdateOrderAsync(currentOrder.Id,currentOrder);
                if (result)
                {
                    var orderResponse = await orderService.GetOrderResponseAsync();
                    if (orderResponse != null)
                    {
                        orderList = orderResponse.Data;
                    }
                    isHiddenPopup = true;
                    isHiddenNotification = false;
                    NotificationDetail = "Update order infomation successfully";
                }
                else
                {
                    isHiddenNotification = false;
                    NotificationDetail = "Failed to update order infomation";
                }
                
            }
        }
    }
}
