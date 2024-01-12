using FurnitureStore.Client.IServices;
using FurnitureStore.Client.Services;
using FurnitureStore.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace FurnitureStore.Client.Pages.AdminPages
{
    public partial class Statistic
    {
        [Inject]
        IProductService productService { get; set; } = null!;
        [Inject]
        IOrderService orderService { get; set; } = null!;
        List<OrderDTO> orderList { get; set; } = new List<OrderDTO>();
        List<ProductDTO> productList { get; set; } = new List<ProductDTO>();
        int TotalProduct { get; set; } = 0;
        int TotalOrder { get; set; } = 0;
        int TotalCustomer { get; set; } = 0;
        int TotalRevenue { get; set; } = 0;
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
            TotalProduct=productList.Count;
            TotalOrder=orderList.Count;
            TotalCustomer = orderList.Select(order => order.CustomerName).Distinct().Count();
            TotalRevenue = orderList.Sum(order => order.TotalAmount);
        }
    }
}
