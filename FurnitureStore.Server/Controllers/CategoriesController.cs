using FurnitureStore.Server.Repositories.Interfaces;

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
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategoriesAsync()
    {
        var categories = await categoryRepository.GetCategoryResponsesAsync();
        
        if (!categories.Any())
        {
            return NotFound();
        }

        return Ok(categories);
    }

    [HttpGet("level/{level}")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesByLevel(int level)
    {
        var categories = await categoryRepository.GetCategoryDTOsByLevelAsync(level);

        if (categories == null || !categories.Any())
        {
            return NotFound();
        }

        return Ok(categories);
    }

    [HttpGet("parent/{parent}")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesByParent(string? parent)
    {
        var categories = await categoryRepository.GetCategoryDTOsByParentAsync(parent);

        if (categories == null || !categories.Any())
        {
            return NotFound();
        }

        return Ok(categories);
    }

    [HttpGet("newId")]
    public async Task<ActionResult<string>> GetNewCategoryIdAsync()
    {
        string newId = await categoryRepository.GetNewCategoryIdAsync();

        return Ok(newId);
    }

    // GET api/<CategoriesController>/5
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

    // POST api/<CategoriesController>
    [HttpPost]
    public async Task<ActionResult> CreateCategoryAsync([FromBody] CategoryDTO categoryDTO)
    {
        try
        {
            await categoryRepository.AddCategoryDTOAsync(categoryDTO);

            return Ok("Category created successfully.");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"An error occurred while creating the category. CategoryId: {categoryDTO.CategoryId}");
        }
    }

    // PUT api/<CategoriesController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategoryAsync(string id, [FromBody] CategoryDTO categoryDTO)
    {
        try
        {
            await categoryRepository.UpdateCategoryAsync(categoryDTO);

            return Ok("Category updated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"An error occurred while creating the category. CategoryId: {categoryDTO.CategoryId}");
        }
    }

    // DELETE api/<CategoriesController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategoryAsync(string id)
    {
        try
        {
            await categoryRepository.DeleteCategoryAsync(id);

            return Ok("Category updated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"{ex.Message}");
        }
    }
}
