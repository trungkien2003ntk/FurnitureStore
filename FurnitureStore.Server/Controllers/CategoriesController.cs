using FurnitureStore.Server.Exceptions;
using FurnitureStore.Server.Models.BindingModels;
using FurnitureStore.Server.Models.BindingModels.FilterModels;
using FurnitureStore.Server.Repositories.Interfaces;
using System.Runtime.InteropServices;

namespace FurnitureStore.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(
    ILogger<CategoriesController> logger,
    ICategoryRepository categoryRepository
) : ControllerBase
{

    // GET: api/<CategoriesController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategoriesAsync(QueryParameters queryParameters, CategoryFilterModel filter)
    {
        IEnumerable<CategoryResponse> categories = [];

        categories = await categoryRepository.GetCategoryResponsesAsync(queryParameters, filter);

        if (!categories.Any())
        {
            return NotFound();
        }

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDTO>> GetCategoryDTOByIdAsync(string id)
    {
        var category = await categoryRepository.GetCategoryDTOByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCategoryAsync([FromBody] CategoryDTO categoryDTO)
    {
        try
        {
            var createdCategoryDTO = await categoryRepository.AddCategoryDTOAsync(categoryDTO);

            if (createdCategoryDTO == null)
            {
                return StatusCode(500, "Failed to create product, please try again");
            }

            return CreatedAtAction(
                nameof(GetCategoryDTOByIdAsync),
                new { id = createdCategoryDTO.Id },
                createdCategoryDTO);
        }
        catch (DuplicateDocumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(
                    $"Create category failed. \n" +
                    $"Error message: {ex.Message}");

            return StatusCode(500,
                $"An error occurred while creating the category. \n" +
                $"Error message: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategoryAsync(string id, [FromBody] CategoryDTO categoryDTO)
    {
        if (id != categoryDTO.CategoryId)
        {
            return BadRequest("Specified id don't match with the DTO.");
        }

        if (categoryDTO == null)
        {
            return BadRequest("CategoryDTO data is needed");
        }

        try
        {
            await categoryRepository.UpdateCategoryAsync(categoryDTO);

            return NoContent();
        }
        catch(DuplicateDocumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(
                    $"Update category failed. " +
                    $"\nCategory Id: {id}. " +
                    $"\nError message: {ex.Message}");

            return StatusCode(500,
                $"An error occurred while updating the category. \n" +
                $"Category Id: {id}\n" +
                $"Error message: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategoryAsync(string id)
    {
        try
        {
            await categoryRepository.DeleteCategoryAsync(id);

            return Ok("Category deleted successfully.");
        }
        catch (DocumentRemovalException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"{ex.Message}");
        }
    }
}