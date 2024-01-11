using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Repositories.Interfaces;
using FurnitureStore.Server.Utils;
using FurnitureStore.Server.Exceptions;

namespace FurnitureStore.Server.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly string NotiCategoryNotFound = "Category not found!";
    private readonly string categoryIdCacheName = "LastestCategoryId";

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
        IMemoryCache memoryCache
    ) 
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

    public async Task<IEnumerable<CategoryResponse>> GetCategoryResponsesAsync(QueryParameters queryParameters, CategoryFilterModel filter)
    {
        List<CategoryResponse> result = [];

        if (filter.UseNestedResult)
        {
            var level1CategoryDocs = await GetCategoryDocumentsByLevelAndParentId(1, "");

            result = (await GetCategoryResponses(level1CategoryDocs, 2)).ToList();
        }
        else
        {
            var queryDef = CosmosDbUtils.BuildQuery(queryParameters, filter, isRemovableDocument:true);

            var categoryDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<CategoryDocument>(_categoryContainer, queryDef);

            result.AddRange(
                categoryDocs
                    .Select(cateDoc => new CategoryResponse() { Category = _mapper.Map<CategoryDTO>(cateDoc)})
                    .ToList());
        }

        return result;
    }

    public async Task<CategoryDTO> GetCategoryDTOByIdAsync(string id)
    {
        var categoryDoc = await GetCategoryDocumentByIdAsync(id) ?? throw new Exception(NotiCategoryNotFound);

        var categoryDTO = _mapper.Map<CategoryDTO>(categoryDoc);

        return categoryDTO;
    }

    public async Task<CategoryDTO?> AddCategoryDTOAsync(CategoryDTO categoryDTO)
    {
        if (await NameExistsInContainer(StringUtils.RemoveAccentsAndHyphenize(categoryDTO.Text)))
        {
            throw new DuplicateDocumentException($"The category {categoryDTO.Text} has already been created. Please choose a different name.");
        }

        var categoryDoc = _mapper.Map<CategoryDocument>(categoryDTO);

        await PopulateDataToNewCategoryDocument(categoryDoc);

        var createdCategoryDoc = await AddCategoryDocumentAsync(categoryDoc);

        if (createdCategoryDoc != null)
        {
            _memoryCache.Set(categoryIdCacheName, IdUtils.IncreaseId(createdCategoryDoc.Id));

            return _mapper.Map<CategoryDTO>(createdCategoryDoc);
        }

        return null;
    }

    public async Task UpdateCategoryAsync(CategoryDTO categoryDTO)
    {
        if (await NameExistsInContainer(StringUtils.RemoveAccentsAndHyphenize(categoryDTO.Text)))
        {
            throw new DuplicateDocumentException($"The category {categoryDTO.Text} has already been created. Please choose a different name.");
        }

        var categoryToUpdate = _mapper.Map<CategoryDocument>(categoryDTO);

        categoryToUpdate.ModifiedAt = DateTime.UtcNow;
        categoryToUpdate.Name = StringUtils.RemoveAccentsAndHyphenize(categoryDTO.Text);
        categoryToUpdate.CategoryPath = categoryToUpdate.ParentPath + "/" + categoryToUpdate.Name;

        await _categoryContainer.UpsertItemAsync(
            item: categoryToUpdate,
            partitionKey: new PartitionKey(categoryToUpdate.ParentPath)
        );
    }

    public async Task DeleteCategoryAsync(string categoryId)
    {
        var categoryDoc = await GetCategoryDocumentByIdAsync(categoryId) ?? throw new Exception(NotiCategoryNotFound);

        // Check if category is deletable
        if (!categoryDoc.IsRemovable)
        {
            throw new DocumentRemovalException("Can't delete this category");
        }

        List<PatchOperation> patchOperations =
        [
            PatchOperation.Replace("/isDeleted", true)
        ];

        await _categoryContainer.PatchItemAsync<CategoryDocument>(categoryId, new PartitionKey(categoryDoc.ParentPath), patchOperations);
    }

    private async Task<IEnumerable<CategoryDocument>> GetCategoryDocumentsByLevelAndParentId(int level, string parentId)
    {
        var queryDef = new QueryDefinition(
            $"SELECT * FROM c WHERE c.level = {level} AND STRINGEQUALS(c.parentId, '{parentId}') AND c.isDeleted = false"
        );
        return await CosmosDbUtils.GetDocumentsByQueryDefinition<CategoryDocument>(_categoryContainer, queryDef);
    }

    private async Task<IEnumerable<CategoryResponse>> GetCategoryResponses(IEnumerable<CategoryDocument> categoryDocs, int nextLevel)
    {
        List<CategoryResponse> responses = [];

        foreach (var categoryDoc in categoryDocs)
        {
            var response = new CategoryResponse()
            {
                Category = _mapper.Map<CategoryDTO>(categoryDoc),
                SubCategories = []
            };

            var nextLevelCategoryDocs = await GetCategoryDocumentsByLevelAndParentId(nextLevel, categoryDoc.Id);
            if (nextLevelCategoryDocs.Any())
            {
                response.SubCategories = (await GetCategoryResponses(nextLevelCategoryDocs, nextLevel + 1)).ToList();
            }
            responses.Add(response);
        }
        return responses;
    }

    private async Task<CategoryDocument?> GetCategoryDocumentByIdAsync(string id)
    {
        var queryDef = new QueryDefinition(
            query:
                "SELECT * " +
                "FROM c " +
                "WHERE c.id = @id AND c.isDeleted = false"
        ).WithParameter("@id", id);

        var category = await CosmosDbUtils.GetDocumentByQueryDefinition<CategoryDocument>(_categoryContainer, queryDef);

        return category;
    }

    private async Task<bool> NameExistsInContainer(string categoryName)
    {
        var queryDef = new QueryDefinition(
            "SELECT * " +
            "FROM c " +
            "WHERE c.isDeleted = false AND STRINGEQUALS(@categoryName,c.name,true)"
        ).WithParameter("@categoryName", categoryName);

        var result = await CosmosDbUtils.GetDocumentByQueryDefinition<CategoryDocument>(_categoryContainer, queryDef);

        return result != null;
    }

    private async Task PopulateDataToNewCategoryDocument(CategoryDocument categoryDoc)
    {
        var parentCate = await GetCategoryDocumentByIdAsync(categoryDoc.ParentId);

        categoryDoc.Id = await GetNewCategoryIdAsync();
        categoryDoc.CategoryId = categoryDoc.Id;
        categoryDoc.Name = StringUtils.RemoveAccentsAndHyphenize(categoryDoc.Text);
        categoryDoc.Children = [];
        categoryDoc.Level = parentCate != null ? parentCate.Level + 1 : 1;
        categoryDoc.ParentPath ??= parentCate != null ? parentCate.CategoryPath : "";
        categoryDoc.CategoryPath = (parentCate != null ? parentCate.CategoryPath : "") + $"/{categoryDoc.Name}";
        categoryDoc.IsRemovable = true;
        categoryDoc.IsDeleted = false;
        categoryDoc.TTL = -1;
    }

    private async Task<CategoryDocument?> AddCategoryDocumentAsync(CategoryDocument item)
    {
        try
        {
            item.CreatedAt = DateTime.UtcNow;
            item.ModifiedAt = item.CreatedAt;

            var response = await _categoryContainer.UpsertItemAsync(
                item: item,
                partitionKey: new PartitionKey(item.ParentPath)
            );

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return response.Resource;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Upsert Item failed, Exception message {ex.Message}");
        }

        return null;
    }

    private async Task<string> GetNewCategoryIdAsync()
    {
        if (_memoryCache.TryGetValue(categoryIdCacheName, out string? lastestId))
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

        _memoryCache.Set(categoryIdCacheName, newId);
        return newId;
    }
}
