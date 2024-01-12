using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Repositories.Interfaces;
using FurnitureStore.Server.Utils;
using FurnitureStore.Server.Exceptions;
using FurnitureStore.Shared.Enums;

namespace FurnitureStore.Server.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string NotiOrderNotFound = "Order Not Found!";
        private readonly string OrderIdCacheName = "LastestOrderId";

        private readonly Container _orderContainer;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<OrderRepository> _logger;

        public int TotalCount { get; private set; }

        public OrderRepository(
            CosmosClient cosmosClient,
            IMapper mapper,
            IMemoryCache memoryCache,
            ILogger<OrderRepository> logger
        )
        {
            var databaseName = cosmosClient.ClientOptions.ApplicationName;
            var containerName = "orders";

            _orderContainer = cosmosClient.GetContainer(databaseName, containerName);
            _mapper = mapper;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDTO>?> GetOrderDTOsAsync(QueryParameters queryParameters, OrderFilterModel filter)
        {
            var queryDef = CosmosDbUtils.BuildQuery(queryParameters, filter, isRemovableDocument: false);

            var orderDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<OrderDocument>(_orderContainer, queryDef);

            queryParameters.PageSize = -1;
            var getAllQueryDef = CosmosDbUtils.BuildQuery(queryParameters, filter, isRemovableDocument: false);
            var allOrderDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<OrderDocument>(_orderContainer, getAllQueryDef);
            TotalCount = allOrderDocs.Count();

            var orderDTOs = orderDocs.Select(orderDoc =>
            {
                return _mapper.Map<OrderDTO>(orderDoc);
            }).ToList();

            return orderDTOs;
        }

        public async Task<OrderDTO?> GetOrderDTOByIdAsync(string id)
        {
            var orderDoc = await GetOrderDocumentByIdAsync(id)
                ?? throw new DocumentNotFoundException(NotiOrderNotFound);

            var orderDTO = _mapper.Map<OrderDTO>(orderDoc);

            return orderDTO;
        }

        public async Task<OrderDTO?> AddOrderDTOAsync(OrderDTO orderDTO)
        {
            var orderDoc = _mapper.Map<OrderDocument>(orderDTO);

            await PopulateDataToNewOrderDocument(orderDoc);

            var createdOrderDoc = await AddOrderDocumentAsync(orderDoc);

            if (createdOrderDoc != null)
            {
                _memoryCache.Set(OrderIdCacheName, IdUtils.IncreaseId(createdOrderDoc.Id));

                return _mapper.Map<OrderDTO>(createdOrderDoc);
            }

            return null;
        }

        public async Task UpdateOrderAsync(OrderDTO orderDTO)
        {
            var orderToUpdate = _mapper.Map<OrderDocument>(orderDTO);

            orderToUpdate.ModifiedAt = DateTime.UtcNow;

            await _orderContainer.UpsertItemAsync(
                item: orderToUpdate,
                partitionKey: new PartitionKey(orderToUpdate.YearMonth)
            );
        }


        private async Task<OrderDocument?> GetOrderDocumentByIdAsync(string id)
        {
            var queryDef = new QueryDefinition(
                query:
                    "SELECT * " +
                    "FROM so " +
                    "WHERE so.id = @id"
            ).WithParameter("@id", id);

            var order = await CosmosDbUtils.GetDocumentByQueryDefinition<OrderDocument>(_orderContainer, queryDef);

            return order;
        }

        private async Task PopulateDataToNewOrderDocument(OrderDocument orderDoc)
        {
            var newId = await GetNewOrderIdAsync();
            orderDoc.Id = newId;
            orderDoc.OrderId = newId;
            orderDoc.CustomerType ??= "Retail";


            orderDoc.Status ??= OrderStatus.created.ToString();
            orderDoc.Note ??= "";
            orderDoc.TTL = -1;
        }

        public async Task<OrderDocument?> AddOrderDocumentAsync(OrderDocument item)
        {
            try
            {
                item.CreatedAt = DateTime.UtcNow;
                item.YearMonth = item.CreatedAt.Value.ToString("yyyy-MM");
                item.ModifiedAt = item.CreatedAt;


                var response = await _orderContainer.UpsertItemAsync(
                    item: item,
                    partitionKey: new PartitionKey(item.YearMonth)
                );

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    return response.Resource;
                }
            }
            catch (CosmosException ex)
            {
                _logger.LogError($"Upsert Item failed, Exception message {ex.Message}");
            }

            return null;
        }

        public async Task<string> GetNewOrderIdAsync()
        {
            if (_memoryCache.TryGetValue(OrderIdCacheName, out string? lastestId))
            {
                if (!string.IsNullOrEmpty(lastestId))
                    return lastestId;
            }

            // Query the database to get the latest product ID
            QueryDefinition queryDef = new QueryDefinition(
                query:
                "SELECT TOP 1 so.id " +
                "FROM so " +
                "ORDER BY so.id DESC"
            );

            string currLastestId = (await CosmosDbUtils.GetDocumentByQueryDefinition<ResponseToGetId>(_orderContainer, queryDef))!.Id;
            string newId = IdUtils.IncreaseId(currLastestId);

            _memoryCache.Set(OrderIdCacheName, newId);
            return newId;
        }
    }
}
