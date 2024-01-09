using FurnitureStore.Shared;

namespace FurnitureStore.Client.Services.Customer
{
    public class ProductService
    {
     
        public Task<ProductDTO[]> GetLatestProducts()
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new ProductDTO
            {
                ProductId = rng.Next(-20, 55).ToString(),
                Name = "Denim shirt",
                RegularPrice = 10000,
            }).ToArray()); 
        }
    }
}
