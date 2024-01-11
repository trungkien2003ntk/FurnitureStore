using FurnitureStore.Server.Repositories.Interfaces;

namespace FurnitureStore.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(
    IOrderRepository orderRepository,
    ILogger<OrdersController> logger
) : ControllerBase
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ILogger<OrdersController> _logger = logger;

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

    [HttpPut]
    public async Task<ActionResult> UpdateOrderAsync([FromBody] OrderDTO orderDTO)
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
