using FurnitureStore.Server.IRepositories;
using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Server.Utils;

namespace FurnitureStore.Server.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string NotiOrderNotFound = "Order Not Found!";
        private readonly string OrderIdCacheName = "LastestOrderId";

        private readonly Container _orderContainer;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public OrderRepository(CosmosClient cosmosClient, IMapper mapper, IMemoryCache memoryCache)
        {
            var databaseName = cosmosClient.ClientOptions.ApplicationName;
            var containerName = "orders";

            _orderContainer = cosmosClient.GetContainer(databaseName, containerName);
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task AddOrderDocumentAsync(OrderDocument item)
        {
            await _orderContainer.UpsertItemAsync(
                item:item,
                partitionKey: new PartitionKey(item.YearMonth)
            );
        }

        public async Task AddOrderDTOAsync(OrderDTO orderDTO)
        {
            var orderDoc = _mapper.Map<OrderDocument>(orderDTO);

            await AddOrderDocumentAsync(orderDoc);

            _memoryCache.Set(OrderIdCacheName, IdUtils.IncreaseId(orderDTO.Id));
        }

        public async Task UpdateOrderAsync(OrderDTO orderDTO)
        {
            var orderToUpdate = _mapper.Map<OrderDocument>(orderDTO);

            await _orderContainer.UpsertItemAsync(
                item: orderToUpdate,
                partitionKey: new PartitionKey(orderToUpdate.YearMonth)
            );
        }

        public async Task<IEnumerable<OrderDTO>> GetOrderDTOsAsync()
        {
            var queryDef = new QueryDefinition(
                query:
                    "SELECT * " +
                    "FROM so"
            );

            var orderDocs = await CosmosDbUtils.GetDocumentsByQueryDefinition<OrderDocument>(_orderContainer, queryDef);
            var orderDTOs = orderDocs.Select(orderDoc =>
            {
                return _mapper.Map<OrderDTO>(orderDoc);
            }).ToList();

            return orderDTOs;
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

        public async Task<OrderDTO> GetOrderDTOByIdAsync(string id)
        {
            var orderDoc = await GetOrderDocumentByIdAsync(id) ?? throw new Exception(NotiOrderNotFound);

            var orderDTO = _mapper.Map<OrderDTO>(orderDoc);

            return orderDTO;
        }

        public async Task DeleteOrderAsync(string id)
        {
            var orderDoc = await GetOrderDocumentByIdAsync(id) ?? throw new Exception(NotiOrderNotFound);

            List<PatchOperation> patchOperations = new List<PatchOperation>()
            {
                PatchOperation.Replace("/isDeleted", true)
            };

            await _orderContainer.PatchItemAsync<OrderDocument>(id, new PartitionKey(orderDoc.YearMonth), patchOperations);
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
    }
}
