using FurnitureStore.Client;
using FurnitureStore.Client.IServices;
using FurnitureStore.Client.IServices.Customer;
using FurnitureStore.Client.Services;
using FurnitureStore.Client.Services.Customer;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using IProductService = FurnitureStore.Client.IServices.Customer.IProductService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
#region trinh: dependency injection
builder.Services.AddScoped<IProductService, FurnitureStore.Client.Services.Customer.ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();


#endregion

await builder.Build().RunAsync();

