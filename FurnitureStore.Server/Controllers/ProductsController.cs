using FurnitureStore.Server.Exceptions;
using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Repositories.Interfaces;
using FurnitureStore.Server.Utils;

namespace FurnitureStore.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(
    IProductRepository productRepository,
    ILogger<ProductsController> logger,
    IValidator<QueryParameters> queryParametersValidator
) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsAsync(QueryParameters queryParameters, ProductFilterModel filter)
    {
        var queryParamResult = queryParametersValidator.Validate(queryParameters);

        if (!queryParamResult.IsValid)
        {
            return BadRequest(queryParamResult.Errors);
        }

        try
        {
            var result = await productRepository.GetProductDTOsAsync(queryParameters, filter);
            var totalCount = productRepository.TotalCount;

            if (VariableHelpers.IsNull(result))
            {
                logger.LogInformation($"No product found!");
                return NotFound();
            }


            logger.LogInformation($"Returned all products!");
            return Ok(new
            {
                data = result,
                metadata = new
                {
                    count = totalCount
                }
            });
        }
        catch (InvalidSortByPropertyException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProductDTOByIdAsync(string id)
    {
        var product = await productRepository.GetProductDTOByIdAsync(id);

        if (product == null)
        {
            logger.LogInformation($"Product with id {id} Not Found");
            return NotFound();
        }

        logger.LogInformation($"Product with id {id} Found");

        return Ok(product);
    }


    [HttpPost]
    public async Task<ActionResult> CreateProductAsync([FromBody] ProductDTO productDTO)
    {
        if (productDTO == null)
        {
            return BadRequest("Product data is required.");
        }

        try
        {
            var createdProductDTO = await productRepository.AddProductDTOAsync(productDTO);


            if (createdProductDTO == null)
            {
                return StatusCode(500, "Failed to create product, please try again");
            }


            return CreatedAtAction(
                   nameof(GetProductDTOByIdAsync), // method
                   new { id = createdProductDTO.ProductId }, // param in method
                   createdProductDTO // values returning after the route
               );
        }
        catch (Exception ex)
        {
            logger.LogError(
                    $"Create product failed. \n" +
                    $"Error message: {ex.Message}");

            return StatusCode(500,
                 $"An error occurred while creating the product. \n" +
                 $"Error message: {ex.Message}");
        }
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProductAsync(string id, [FromBody] ProductDTO productDTO)
    {
        if (productDTO == null)
        {
            return BadRequest("Product data is required.");
        }

        if (id != productDTO.Id)
        {
            return BadRequest("Specified id don't match with the DTO");
        }

        try
        {
            await productRepository.UpdateProductDTOAsync(productDTO);

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(
                    $"Updating product failed. " +
                    $"\nProduct Id: {id}. " +
                    $"\nError message: {ex.Message}");

            return StatusCode(500,
                $"An error occurred while updating the product. \n" +
                $"Product Id: {id}\n" +
                $"Error message: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductAsync(string id)
    {
        try
        {
            await productRepository.DeleteProductDTOAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(
                    $"Delete product failed. \n" +
                    $"Product Id: {id}. \n" +
                    $"Error message: {ex.Message}");

            return StatusCode(500,
                $"An error occurred while deleting the product. \n" +
                $"Product Id: {id}\n" +
                $"Error message: {ex.Message}");
        }
    }
}