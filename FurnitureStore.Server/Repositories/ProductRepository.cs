using FurnitureStore.Server.Exceptions;
using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Repositories.Interfaces;
using FurnitureStore.Server.Services;
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
        private readonly ICategoryRepository _categoryRepository;
        private CategoryDocument? _defaultCategoryDoc;
        private readonly AzureSearchClientService _searchService;


        public int TotalCount { get; private set; } = 0;

        public ProductRepository(
            CosmosClient cosmosClient,
            ILogger<CategoryRepository> logger,
            IMemoryCache memoryCache,
            IMapper mapper,
            ICategoryRepository categoryRepository,
            AzureSearchServiceFactory searchServiceFactory
        )
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _mapper = mapper;
            var databaseName = cosmosClient.ClientOptions.ApplicationName;

            _productContainer = cosmosClient.GetContainer(databaseName, "products");
            _categoryContainer = cosmosClient.GetContainer(databaseName, "categories");
            _categoryRepository = categoryRepository;

            _searchService = searchServiceFactory.Create("products");
        }

        public async Task<IEnumerable<ProductDTO>> GetProductDTOsAsync(QueryParameters queryParameters, ProductFilterModel filter)
        {
            IEnumerable<ProductDocument> productDocs = [];

            if (!string.IsNullOrEmpty(filter.Query))
            {
                var options = AzureSearchUtils.BuildOptions(queryParameters, filter);
                var searchResult = await _searchService.SearchAsync<ProductDocument>(filter.Query, options);
                TotalCount = searchResult.TotalCount;

                productDocs = searchResult.Results;

            }
            else
            {
                var queryDef = CosmosDbUtils.BuildQuery(queryParameters, filter, isRemovableDocument: true);
                productDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<ProductDocument>(_productContainer, queryDef);
            }



            if (!VariableHelpers.IsNull(filter.CategoryIds))
            {
                var tasks = filter.CategoryIds!.Select(async cateId => {
                    try
                    {
                        var category = await _categoryRepository.GetCategoryDTOByIdAsync(cateId);
                        
                        
                        return category.CategoryPath;

                    }
                    catch (DocumentNotFoundException)
                    {
                        return null;
                    }
                });

                var categoryPaths = await Task.WhenAll(tasks);

                productDocs = productDocs.Where(p =>
                {
                    var paths = p.CategoryPath.Split('/').Skip(1).ToList(); // Skip the first empty string from split
                    for (int i = 0; i < paths.Count; i++)
                    {
                        var pathToCheck = string.Join("/", paths.Take(i + 1));
                        if (categoryPaths.Contains($"/{pathToCheck}"))
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }

            if (!VariableHelpers.IsNull(filter.PriceRangeStrings))
            {
                var priceRanges = filter.PriceRanges!;
                productDocs = productDocs.Where(p =>
                    priceRanges.Any(range => (p.SalePrice >= range.MinPrice) && (p.SalePrice <= range.MaxPrice))
                ).ToList();
            }

            List<ProductDocument> filteredProducts = [];

            if (!VariableHelpers.IsNull(filter.VariationId))
            {
                productDocs = productDocs.Where(p => filter.VariationId == p.VariationDetail.Id);

                filteredProducts.AddRange(productDocs);
            }
            else
            {
                // Create a dictionary to store the product with the lowest salePrice for each variationId
                var lowestPriceVariationProducts = productDocs
                    .Where(p => p.VariationDetail.Id != null)
                    .GroupBy(p => p.VariationDetail.Id!)
                    .ToDictionary(
                        group => group.Key,
                        group => group.OrderBy(p => p.SalePrice).First()
                    );

                // Iterate over productDocs in the original order
                foreach (var product in productDocs)
                {
                    if (product.VariationDetail?.Id == null)
                    {
                        // If variationId is null, add the product to filteredProducts
                        filteredProducts.Add(product);
                    }
                    else if (lowestPriceVariationProducts.TryGetValue(product.VariationDetail.Id, out var lowestPriceProduct))
                    {
                        // If this product is the one with the lowest salePrice for its variationId, add it to filteredProducts
                        if (product == lowestPriceProduct)
                        {
                            filteredProducts.Add(product);
                        }
                    }
                }
            }
            
            TotalCount = filteredProducts.Count;

            

            if (!string.IsNullOrEmpty(queryParameters.SortBy) && string.IsNullOrEmpty(filter.Query))
            {
                var propertyName = VariableHelpers.ToCamelCase(queryParameters.SortBy);
                var property = typeof(ProductDocument).GetProperty(propertyName);
                if (property != null)
                {
                    if (queryParameters.OrderBy?.ToLower() == "desc")
                    {
                        filteredProducts = filteredProducts.OrderByDescending(p => property.GetValue(p, null)).ToList();
                    }
                    else
                    {
                        filteredProducts = filteredProducts.OrderBy(p => property.GetValue(p, null)).ToList();
                    }
                }
                else
                {
                    throw new InvalidSortByPropertyException(queryParameters.SortBy);
                }
            }

            // pagination
            if (queryParameters.PageSize.HasValue && queryParameters.PageSize != -1)
            {
                int skip = (queryParameters.PageNumber.GetValueOrDefault(1) - 1) * queryParameters.PageSize.Value;
                filteredProducts = filteredProducts.Skip(skip).Take(queryParameters.PageSize.Value).ToList();
            }

            var productDTOs = filteredProducts.Select(productDoc =>
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

            if (createdProductDoc != null) 
            {
                _memoryCache.Set(cacheProductNewIdName, IdUtils.IncreaseId(createdProductDoc.Id));

                _logger.LogInformation($"Product and inventory with id {productDoc.Id} added");
                
                return _mapper.Map<ProductDTO>(createdProductDoc);
            }
            
            return null;
        }

        public async Task UpdateProductDTOAsync(ProductDTO productDTO)
        {
            var productDoc = _mapper.Map<ProductDocument>(productDTO);


            productDoc.Status = DocumentStatusUtils.GetInventoryStatus(
                productDoc.Stock,
                productDoc.MinStock
            );

            productDoc.IsRemovable = true;
            productDoc.IsDeleted = false;
            productDoc.ModifiedAt = DateTime.UtcNow;





            await _productContainer.UpsertItemAsync(
                    item: productDoc,
                    partitionKey: new PartitionKey(productDoc.Sku)
                );
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

            productDoc.MinStock ??= 3;
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
                item.CreatedAt = DateTime.UtcNow;
                item.ModifiedAt = item.CreatedAt;

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
