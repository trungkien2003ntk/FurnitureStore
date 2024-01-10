using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Models.Documents;
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

        var result = await productRepository.GetProductDTOsAsync(queryParameters, filter);

        if (VariableHelpers.IsNull(result))
        {
            logger.LogInformation($"No product found!");
            return NotFound();
        }

        logger.LogInformation($"Returned all products!");
        return Ok(result);
    }


    [HttpGet("variation/{variationId}")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductDTOsByVariationId(string variationId)
    {
        var products = await productRepository.GetProductDTOsByVariationIdAsync(variationId);

        if (products == null || !products.Any()) 
        {
            return NotFound();
        }

        return Ok(products);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDocument>>> GetProductsInCategoryAsync(string categoryId)
    {
        var products = await productRepository.GetProductDocumentsInCategoryAsync(categoryId);

        if (products == null || !products.Any())
        {
            logger.LogInformation($"No product found in category id {categoryId}!");
            return NotFound();
        }

        logger.LogInformation($"Returned all products in category id {categoryId}!");
        return Ok(products);
    }


    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<ProductDTO>> GetProductBySkuAsync(string sku)
    {
        var product = await productRepository.GetProductDTOBySkuAsync(sku);

        if (product == null)
        {
            logger.LogInformation($"Product with sku {sku} Not Found");
            return NotFound();
        }

        logger.LogInformation($"Product with sku {sku} Found");

        return Ok(product);
    }

    [HttpGet("newId")]
    public async Task<ActionResult<string>> GetNewProductIdAsync()
    {
        string newId = await productRepository.GetNewProductIdAsync();

        return Ok(newId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProductByIdAsync(string id)
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
            await productRepository.AddProductDTOAsync(productDTO);

            return Ok("Product created successfully.");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"An error occurred while creating the product. ProductId: {productDTO.ProductId}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProductAsync(string id, [FromBody] ProductDTO productDTO)
    {
        if (productDTO == null)
        {
            return BadRequest("Product data is required.");
        }

        try
        {
            await productRepository.UpdateProductDTOAsync(productDTO);

            return Ok("Product updated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"An error occurred while updating the product. ProductId: {productDTO.ProductId}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProductAsync(string id)
    {
        try
        {
            await productRepository.DeleteProductDTOAsync(id);

            return Ok("Product deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"An error occurred while deleting the product. Product Id: {id}");
        }
    }
}
