using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Repositories.Interfaces;
using FurnitureStore.Server.Utils;
using System.Runtime.CompilerServices;

namespace FurnitureStore.Server.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string NotiProductNotFound = "Product Not Found!";

        private readonly ILogger<CategoryRepository> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;
        private readonly Container _productContainer;

        public ProductRepository(CosmosClient cosmosClient, ILogger<CategoryRepository> logger, IMemoryCache memoryCache, IMapper mapper)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _mapper = mapper;
            var databaseName = cosmosClient.ClientOptions.ApplicationName;

            _productContainer = cosmosClient.GetContainer(databaseName, "products");
        }

        public async Task AddProductDocumentAsync(ProductDocument item)
        {
            await _productContainer.UpsertItemAsync(
                item: item,
                partitionKey: new PartitionKey(item.Sku)
            );
        }

        public async Task AddProductDTOAsync(ProductDTO productDTO)
        {
            var productDoc = _mapper.Map<ProductDocument>(productDTO);


            await AddProductDocumentAsync(productDoc);

            _memoryCache.Set("LatestProductId", IdUtils.IncreaseId(productDoc.Id));

            _logger.LogInformation($"Product and inventory with id {productDoc.Id} added");
        }

        public async Task UpdateProductDTOAsync(ProductDTO productDTO)
        {
            var productDoc = _mapper.Map<ProductDocument>(productDTO);

            await AddProductDocumentAsync(productDoc);

            _logger.LogInformation($"Product and inventory with id {productDoc.Id} added");
        }

        public async Task<string> GetNewProductIdAsync()
        {
            if (_memoryCache.TryGetValue("LatestProductId", out string? lastestId))
            {
                if (!String.IsNullOrEmpty(lastestId))
                    return lastestId;
            }

            // Query the database to get the latest product ID
            QueryDefinition queryDef = new QueryDefinition(
                query: 
                "SELECT TOP 1 p.id " +
                "FROM p " +
                "WHERE p.isDeleted = false " +
                "ORDER BY p.id DESC"
            );

            string currLastestId = (await CosmosDbUtils.GetDocumentByQueryDefinition<ResponseToGetId>(_productContainer, queryDef))!.Id;
            string newId = IdUtils.IncreaseId(currLastestId);

            _memoryCache.Set("LatestProductId", newId);
            return newId;
        }

        public async Task<IEnumerable<ProductDocument>> GetProductDocumentsAsync()
        {
            var queryDef = new QueryDefinition(
                query: 
                "SELECT * " +
                "FROM products p " +
                "WHERE p.isDeleted = false"
            );

            var results = await CosmosDbUtils.GetDocumentsByQueryDefinition<ProductDocument>(_productContainer, queryDef);

            return results;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductDTOsAsync(QueryParameters queryParameters, ProductFilterModel filter)
        {
            var queryDef = CosmosDbUtils.BuildQuery(queryParameters, filter, isRemovableDocument: true);

            var productDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<ProductDocument>(_productContainer, queryDef);

            var productDTOs = productDocs.Select(productDoc =>
            {
                return _mapper.Map<ProductDTO>(productDoc);
            }).ToList();

            return productDTOs;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductDTOsByVariationIdAsync(string variationId)
        {
            var queryDef = new QueryDefinition(
                query:
                "SELECT * " +
                "FROM p " +
                "WHERE p.isDeleted = false " +
                "   AND STRINGEQUALS(p.variationDetail.id, @variationId"
            ).WithParameter("@variationId", variationId);

            var result = await CosmosDbUtils.GetDocumentsByQueryDefinition<ProductDocument>(_productContainer, queryDef);


            return result.Select(prodDoc => _mapper.Map<ProductDTO>(prodDoc));
        }


        public async Task<IEnumerable<ProductDocument>> GetProductDocumentsInCategoryAsync(string categoryId)
        {
            var queryDef = new QueryDefinition(
                query: 
                "SELECT * " +
                "FROM products p " +
                "WHERE p.categoryId = @categoryId"
            ).WithParameter("@categoryId", categoryId);

            var results = await CosmosDbUtils.GetDocumentsByQueryDefinition<ProductDocument>(_productContainer, queryDef);

            return results;
        }

        public async Task<ProductDTO?> GetProductDTOBySkuAsync(string sku)
        {
            var productDoc = await GetProductDocumentBySkuAsync(sku) ?? throw new Exception(NotiProductNotFound);

            var result = _mapper.Map<ProductDTO>(productDoc);

            return result;
        }

        private async Task<ProductDocument?> GetProductDocumentBySkuAsync(string sku)
        {
            var queryDef = new QueryDefinition(
                query: "SELECT * " +
                "FROM p " +
                "WHERE p.sku = @sku"
            ).WithParameter("@sku", sku);

            var result = await CosmosDbUtils.GetDocumentByQueryDefinition<ProductDocument>(_productContainer, queryDef);

            return result;
        }

        public async Task<ProductDTO?> GetProductDTOByIdAsync(string id)
        {
            var productDoc = await GetProductDocumentByIdAsync(id);

            var result = _mapper.Map<ProductDTO>(productDoc);

            return result;
        }

        private async Task<ProductDocument> GetProductDocumentByIdAsync(string id)
        {
            var queryDef = new QueryDefinition(
                query: "SELECT * " +
                "FROM products p " +
                "WHERE p.id = @id AND p.isDeleted = false"
            ).WithParameter("@id", id);

            var result = await CosmosDbUtils.GetDocumentByQueryDefinition<ProductDocument>(_productContainer, queryDef);

            return result;
        }

        private void LogRequestCharged(double requestCharge)
        {
            _logger.LogInformation($"Request charged: {requestCharge}");
        }

        public async Task DeleteProductDTOAsync(string id)
        {
            var productDoc = await GetProductDocumentByIdAsync(id) ?? throw new Exception(NotiProductNotFound);

            List<PatchOperation> operations =
            [
                PatchOperation.Replace("/isDeleted", true)
            ];

            await _productContainer.PatchItemAsync<dynamic>(productDoc.Id, new PartitionKey(productDoc.Sku), operations);
        }
    }
}
