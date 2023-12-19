using Microsoft.AspNetCore.Mvc;
using FurnitureStore.Shared;
using FurnitureStore.Server.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FurnitureStore.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
        {
            this._orderRepository = orderRepository;
            this._logger = logger;
        }

        // GET: api/<OrdersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrderDTOsAsync()
        {
            var orders = await _orderRepository.GetOrderDTOsAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return Ok(orders);
        }

        [HttpGet("newId")]
        public async Task<ActionResult<string>> GetNewOrderIdAsync()
        {
            string newId = await _orderRepository.GetNewOrderIdAsync();

            return Ok(newId);
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrderDTOByIdAsync(string id)
        {
            var order = await _orderRepository.GetOrderDTOByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<ActionResult> CreateOrderAsync([FromBody] OrderDTO orderDTO)
        {
            try
            {
                await _orderRepository.AddOrderDTOAsync(orderDTO);

                return Ok("Order created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error message: {ex.Message}");
                return StatusCode(500, $"An error occurred while creating the order. OrderId: {orderDTO.OrderId}");
            }
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrderAsync(string id, [FromBody] OrderDTO orderDTO)
        {
            try
            {
                await _orderRepository.UpdateOrderAsync(orderDTO);

                return Ok("Order updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error message: {ex.Message}");
                return StatusCode(500, $"An error occurred while creating the order. OrderId: {orderDTO.OrderId}");
            }
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrderAsync(string id)
        {
            try
            {
                await _orderRepository.DeleteOrderAsync(id);

                return Ok("Order updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error message: {ex.Message}");
                return StatusCode(500, $"An error occurred while creating the order. OrderId: {id}");
            }
        }
    }
}
