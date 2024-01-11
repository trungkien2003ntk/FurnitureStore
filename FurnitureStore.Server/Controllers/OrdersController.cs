using FurnitureStore.Server.Exceptions;
using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Repositories.Interfaces;
using FurnitureStore.Shared.DTOs;

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
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrderDTOsAsync(QueryParameters queryParameters, OrderFilterModel filter)
    {
        var orders = await _orderRepository.GetOrderDTOsAsync(queryParameters, filter);
        var totalCount = _orderRepository.TotalCount;

        if (orders == null || !orders.Any())
        {
            return NotFound();
        }

        return Ok(new OrderResponse()
        {
            Data = orders.ToList(),
            Metadata = new Metadata() { Count = totalCount }
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDTO>> GetOrderDTOByIdAsync(string id)
    {
        try
        {
            var order = await _orderRepository.GetOrderDTOByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
        catch(DocumentNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<OrderDTO>> CreateOrderAsync([FromBody] OrderDTO orderDTO)
    {
        try
        {
            var createdOrderDTO = await _orderRepository.AddOrderDTOAsync(orderDTO);

            if (createdOrderDTO == null)
            {
                return StatusCode(500, "Failed to create order, please try again");
            }

            return CreatedAtAction(
                nameof(GetOrderDTOByIdAsync),
                new {id = createdOrderDTO.Id},
                createdOrderDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(
                $"Create order failed. \n" +
                $"Error message: {ex.Message}");

            return StatusCode(500,
                $"An error occurred while creating the order. \n" +
                $"Error message: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateOrderAsync(string id, [FromBody] OrderDTO orderDTO)
    {
        if (id != orderDTO.OrderId)
        {
            return BadRequest("Specified id don't match with the DTO.");
        }

        if (orderDTO == null)
        {
            return BadRequest("OrderDTO data is needed");
        }

        try
        {
            await _orderRepository.UpdateOrderAsync(orderDTO);

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(
                $"Update Order failed. \n" +
                $"Order Id: {id}. \n" +
                $"Error message: {ex.Message}");

            return StatusCode(500,
                $"An error occurred while updating the Order. \n" +
                $"Order Id: {id}\n" +
                $"Error message: {ex.Message}");
        }
    }
}
