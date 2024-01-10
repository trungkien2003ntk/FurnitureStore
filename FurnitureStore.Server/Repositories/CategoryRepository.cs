using FurnitureStore.Server.IRepositories;
using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Utils;

namespace FurnitureStore.Server.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly string NotiCategoryNotFound = "Category not found!";
    private readonly string CategoryIdCacheName = "LastestCategoryId";

    private readonly ILogger<CategoryRepository> _logger;
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly Container _categoryContainer;
    private readonly Container _productContainer;

    public CategoryRepository(
        CosmosClient cosmosClient, 
        ILogger<CategoryRepository> logger, 
        IMapper mapper, 
        IProductRepository productRepository, 
        IMemoryCache memoryCache) 
    {
        _logger = logger;
        _mapper = mapper;
        _productRepository = productRepository;
        _memoryCache = memoryCache;
        var databaseName = cosmosClient.ClientOptions.ApplicationName;
        var containerName = "categories";

        _categoryContainer = cosmosClient.GetContainer(databaseName, containerName);
        _productContainer = cosmosClient.GetContainer(databaseName, "products");
    }

    public async Task AddCategoryDocumentAsync(CategoryDocument item)
    {
        try
        {
            var response = await _categoryContainer.UpsertItemAsync(
                item: item,
                partitionKey: new PartitionKey(item.Parent)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Upsert Item failed, Exception message {ex.Message}"); 
        }
    }

    public async Task<CategoryDTO> GetCategoryDTOByIdAsync(string id)
    {
        var categoryDoc = await GetCategoryDocumentByIdAsync(id) ?? throw new Exception(NotiCategoryNotFound);

        var categoryDTO = _mapper.Map<CategoryDTO>(categoryDoc);

        return categoryDTO;
    }

    public async Task<IEnumerable<CategoryDTO>> GetCategoryDTOsAsync()
    {
        var queryDef = new QueryDefinition(
            query:
                "SELECT * " +
                "FROM c"
        );

        var categoryDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<CategoryDocument>(_categoryContainer, queryDef);
        var categoryDTOs = categoryDocs.Select(categoryDoc =>
        {
            return _mapper.Map<CategoryDTO>(categoryDoc);
        }).ToList();

        return categoryDTOs;
    }

    public async Task AddCategoryDTOAsync(CategoryDTO categoryDTO)
    {
        var categoryDoc = _mapper.Map<CategoryDocument>(categoryDTO);

        await AddCategoryDocumentAsync(categoryDoc);

        _memoryCache.Set(CategoryIdCacheName, IdUtils.IncreaseId(categoryDTO.Id));
    }
    
    public async Task UpdateCategoryAsync(CategoryDTO item)
    {
        var categoryToUpdate = _mapper.Map<CategoryDocument>(item);

        await _categoryContainer.UpsertItemAsync(
            item: categoryToUpdate,
            partitionKey: new PartitionKey(categoryToUpdate.Parent)
        );
    }

    public async Task DeleteCategoryAsync(string categoryId)
    {
        var categoryDoc = await GetCategoryDocumentByIdAsync(categoryId) ?? throw new Exception(NotiCategoryNotFound);

        // Check if category is deletable
        if (!categoryDoc.IsDeletable)
        {
            throw new Exception("Can't delete this category");
        }

        List<PatchOperation> patchOperations =
        [
            PatchOperation.Replace("/isDeleted", true)
        ];

        await _categoryContainer.PatchItemAsync<CategoryDocument>(categoryId, new PartitionKey(categoryDoc.CategoryId), patchOperations);
    }

    private async Task<CategoryDocument?> GetCategoryDocumentByIdAsync(string id)
    {
        var queryDef = new QueryDefinition(
            query:
                "SELECT * " +
                "FROM c " +
                "WHERE c.id = @id"
        ).WithParameter("@id", id);

        var category = await CosmosDbUtils.GetDocumentByQueryDefinition<CategoryDocument>(_categoryContainer, queryDef);

        return category;
    }

    private async Task<CategoryDocument?> GetCategoryDocumentByNameAsync(string name)
    {
        var queryDef = new QueryDefinition(
            query:
                "SELECT * " +
                "FROM c " +
                "WHERE c.name = @name"
        ).WithParameter("@name", name);

        var category = await CosmosDbUtils.GetDocumentByQueryDefinition<CategoryDocument>(_categoryContainer, queryDef);

        return category;
    }

    public async Task<string> GetNewCategoryIdAsync()
    {
        if (_memoryCache.TryGetValue(CategoryIdCacheName, out string? lastestId))
        {
            if (!String.IsNullOrEmpty(lastestId))
                return lastestId;
        }

        // Query the database to get the latest product ID
        QueryDefinition queryDef = new QueryDefinition(
            query:
            "SELECT TOP 1 c.id " +
            "FROM c " +
            "ORDER BY c.id DESC"
        );

        string currLastestId = (await CosmosDbUtils.GetDocumentByQueryDefinition<ResponseToGetId>(_categoryContainer, queryDef))!.Id;
        string newId = IdUtils.IncreaseId(currLastestId);

        _memoryCache.Set(CategoryIdCacheName, newId);
        return newId;
    }

    public async Task<IEnumerable<CategoryDTO>> GetCategoryDTOsByLevelAsync(int level)
    {
        var queryDef = new QueryDefinition(
            query:
                "SELECT * " +
                "FROM categories c " +
                "WHERE c.level = @level"
        ).WithParameter("@level", level);

        var categoryDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<CategoryDocument>(_categoryContainer, queryDef);
        var categoryDTOs = categoryDocs.Select(categoryDoc =>
        {
            return _mapper.Map<CategoryDTO>(categoryDoc);
        }).ToList();

        return categoryDTOs;
    }
}
