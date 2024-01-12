using FurnitureStore.Client;
using FurnitureStore.Client.IServices.Customer;
using FurnitureStore.Client.Services.Customer;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FurnitureStore.Client.IServices;
using FurnitureStore.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
#region trinh: dependency injection
builder.Services.AddScoped<IProductService, ProductService>();
#endregion

await builder.Build().RunAsync();

