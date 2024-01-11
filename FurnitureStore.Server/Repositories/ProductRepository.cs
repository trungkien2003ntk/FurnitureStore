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
        private readonly ICategoryRepository _categoryRepository;
        private CategoryDocument? _defaultCategoryDoc;


        public int TotalCount { get; private set; } = 0;

        public ProductRepository(CosmosClient cosmosClient, ILogger<CategoryRepository> logger, IMemoryCache memoryCache, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _mapper = mapper;
            var databaseName = cosmosClient.ClientOptions.ApplicationName;

            _productContainer = cosmosClient.GetContainer(databaseName, "products");
            _categoryContainer = cosmosClient.GetContainer(databaseName, "categories");
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductDTOsAsync(QueryParameters queryParameters, ProductFilterModel filter)
        {
            
            var queryDef = CosmosDbUtils.BuildQuery(queryParameters, filter, isRemovableDocument: true);
            var productDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<ProductDocument>(_productContainer, queryDef);

            List<ProductDocument> filteredProducts = [];

            if (!VariableHelpers.IsNull(filter.CategoryIds))
            {
                var tasks = filter.CategoryIds!.Select(async cateId => {
                    var category = await _categoryRepository.GetCategoryDTOByIdAsync(cateId);

                    if (category != null)
                    {
                        return category.CategoryPath;
                    }

                    return null;
                });

                var categoryPaths = await Task.WhenAll(tasks);

                productDocs = productDocs.Where(p =>
                {
                    var paths = p.CategoryPath!.Split('/');

                    return paths.Any(path => categoryPaths.Contains($"/{path}")) ||
                           paths.Skip(1).Any(path => categoryPaths.Contains($"/{paths.First()}/{path}")) ||
                           paths.Skip(2).Any(path => categoryPaths.Contains($"/{paths.First()}/{paths.Skip(1).First()}/{path}"));
                });
            }

            if (!VariableHelpers.IsNull(filter.VariationId))
            {
                productDocs = productDocs.Where(p => filter.VariationId == p.VariationDetail.Id);

                filteredProducts.AddRange(productDocs);
            }
            else
            {
                // Group the products by variationDetails.id
                var groupedProducts = productDocs
                    .Where(p => p.VariationDetail?.Id != null)
                    .GroupBy(p => p.VariationDetail.Id);

                // Select the product with the lowest salePrice from each group
                foreach (var group in groupedProducts)
                {
                    var productWithLowestPrice = group.OrderBy(p => p.SalePrice).First();
                    filteredProducts.Add(productWithLowestPrice);
                }

                // Include products where variationDetails.id is null
                filteredProducts.AddRange(productDocs.Where(p => p.VariationDetail?.Id == null));
            }
            


            TotalCount = filteredProducts.Count;

            

            if (!string.IsNullOrEmpty(queryParameters.SortBy))
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
