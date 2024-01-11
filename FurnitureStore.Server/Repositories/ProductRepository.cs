using FurnitureStore.Server.Exceptions;
using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Repositories.Interfaces;
using FurnitureStore.Server.Utils;
using Microsoft.Azure.Cosmos.Core.Collections;
using System.Runtime.CompilerServices;

namespace FurnitureStore.Server.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string NotiProductNotFound = "Product Not Found!";
        private readonly string cacheProductNewIdName = "LatestProductId";
        private readonly ILogger<CategoryRepository> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;
        private readonly Container _productContainer;
        private readonly Container _categoryContainer;
        private CategoryDocument? _defaultCategoryDoc;


        public int TotalCount { get; private set; } = 0;

        public ProductRepository(CosmosClient cosmosClient, ILogger<CategoryRepository> logger, IMemoryCache memoryCache, IMapper mapper)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _mapper = mapper;
            var databaseName = cosmosClient.ClientOptions.ApplicationName;

            _productContainer = cosmosClient.GetContainer(databaseName, "products");
            _categoryContainer = cosmosClient.GetContainer(databaseName, "categories");
        }

        public async Task<IEnumerable<ProductDTO>> GetProductDTOsAsync(QueryParameters queryParameters, ProductFilterModel filter)
        {
            var queryDef = CosmosDbUtils.BuildQuery(queryParameters, filter, isRemovableDocument: true);

            queryParameters.PageSize = -1;
            var getAllQueryDef = CosmosDbUtils.BuildQuery(queryParameters, filter, isRemovableDocument: true);

            var productDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<ProductDocument>(_productContainer, queryDef);
            TotalCount = (await CosmosDbUtils.GetDocumentsByQueryDefinition<ProductDocument>(_productContainer, getAllQueryDef)).Count();

            var productDTOs = productDocs.Select(productDoc =>
            {
                return _mapper.Map<ProductDTO>(productDoc);
            }).ToList();

            return productDTOs;
        }

        public async Task<ProductDTO?> GetProductDTOByIdAsync(string id)
        {
            var productDoc = await GetProductDocumentByIdAsync(id);

            var result = _mapper.Map<ProductDTO>(productDoc);

            return result;
        }

        public async Task<ProductDTO?> AddProductDTOAsync(ProductDTO productDTO)
        {
            var productDoc = _mapper.Map<ProductDocument>(productDTO);

            await PopulateDataToNewDocument(productDoc);

            var createdProductDoc = await AddProductDocumentAsync(productDoc);

            if (!VariableHelpers.IsNull(createdProductDoc)) 
            {
                _memoryCache.Set(cacheProductNewIdName, IdUtils.IncreaseId(productDoc.Id));

                _logger.LogInformation($"Product and inventory with id {productDoc.Id} added");
                
                return _mapper.Map<ProductDTO>(createdProductDoc);
            }
            
            return null;
        }

        public async Task UpdateProductDTOAsync(ProductDTO productDTO)
        {
            var productDoc = _mapper.Map<ProductDocument>(productDTO);

            await AddProductDocumentAsync(productDoc);

            _logger.LogInformation($"Product and inventory with id {productDoc.Id} added");
        }

        public async Task DeleteProductDTOAsync(string id)
        {
            var productDoc = await GetProductDocumentByIdAsync(id) 
                ?? throw new DocumentNotFoundException(NotiProductNotFound);

            List<PatchOperation> operations =
            [
                PatchOperation.Replace("/isDeleted", true)
            ];

            await _productContainer.PatchItemAsync<dynamic>(productDoc.Id, new PartitionKey(productDoc.Sku), operations);
        }

        private async Task<ProductDocument?> GetProductDocumentByIdAsync(string id)
        {
            var queryDef = new QueryDefinition(
                query: "SELECT * " +
                "FROM products p " +
                "WHERE p.id = @id AND p.isDeleted = false"
            ).WithParameter("@id", id);

            var result = await CosmosDbUtils.GetDocumentByQueryDefinition<ProductDocument>(_productContainer, queryDef);

            return result;
        }

        private async Task PopulateDataToNewDocument(ProductDocument productDoc)
        {
            _defaultCategoryDoc ??= await GetDefaultCategoryDocument();
            var productId = await GetNewProductIdAsync();

            productDoc.Id = productId;
            productDoc.ProductId = productId;
            productDoc.CategoryId ??= _defaultCategoryDoc.Id;
            productDoc.CategoryPath ??= _defaultCategoryDoc.CategoryPath;
            productDoc.CategoryName ??= _defaultCategoryDoc.Text;
            productDoc.Description ??= "";
            productDoc.Sku ??= productId;

            productDoc.MinStock ??= 0;
            productDoc.Status = DocumentStatusUtils.GetInventoryStatus(
                productDoc.Stock,
                productDoc.MinStock
            );
            productDoc.IsRemovable = true;
            productDoc.IsDeleted = false;
        }

        private async Task<ProductDocument?> AddProductDocumentAsync(ProductDocument item)
        {
            try
            {
                var response = await _productContainer.UpsertItemAsync(
                    item: item,
                    partitionKey: new PartitionKey(item.Sku)
                );

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return response.Resource;
                }
            }
            catch (Exception)
            {
                _logger.LogError("Failed to create product. Please check the id or PartitionKey again");
            }

            return null;
        }

        private async Task<CategoryDocument> GetDefaultCategoryDocument()
        {
            var getDefaultCategoryQueryDef = new QueryDefinition(
                   query:
                   "SELECT * " +
                   "FROM c " +
                   "WHERE c.id = @id"
               ).WithParameter("@id", "cate00000");

            var result = (await CosmosDbUtils.GetDocumentByQueryDefinition<CategoryDocument>(_categoryContainer, getDefaultCategoryQueryDef))!;

            return result;
        }

        private async Task<string> GetNewProductIdAsync()
        {
            if (_memoryCache.TryGetValue(cacheProductNewIdName, out string? lastestId))
            {
                if (!string.IsNullOrEmpty(lastestId))
                    return lastestId;
            }

            // Query the database to get the latest product ID
            QueryDefinition queryDef = new QueryDefinition(
                query: 
                "SELECT TOP 1 p.id " +
                "FROM p " +
                "ORDER BY p.id DESC"
            );

            string currLastestId = (await CosmosDbUtils.GetDocumentByQueryDefinition<ResponseToGetId>(_productContainer, queryDef))!.Id;
            string newId = IdUtils.IncreaseId(currLastestId);

            _memoryCache.Set(cacheProductNewIdName, newId);
            return newId;
        }
    }
}
