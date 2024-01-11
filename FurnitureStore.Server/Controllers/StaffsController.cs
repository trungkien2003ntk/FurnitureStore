using FurnitureStore.Server.Repositories.Interfaces;

namespace FurnitureStore.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StaffsController(
    ILogger<StaffsController> logger,
    IStaffRepository staffRepository
) : ControllerBase
{

    // GET: api/<StaffsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StaffDTO>>> GetStaffDTOsAsync()
    {
        var staffs = await staffRepository.GetStaffDTOsAsync();

        if (staffs == null || !staffs.Any())
        {
            return NotFound();
        }

        return Ok(staffs);
    }

    [HttpGet("newId")]
    public async Task<ActionResult<string>> GetNewStaffIdAsync()
    {
        string newId = await staffRepository.GetNewStaffIdAsync();

        return Ok(newId);
    }

    // GET api/<StaffsController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StaffDTO>> GetStaffDTOByIdAsync(string id)
    {
        var staff = await staffRepository.GetStaffDTOByIdAsync(id);

        if (staff == null)
        {
            return NotFound();
        }

        return Ok(staff);
    }

    [HttpGet("{username}/{password}")]
    public async Task<ActionResult<StaffDTO>> LoginStaff(string username, string password)
    {
        var staff = await staffRepository.LoginStaff(username, password);

        if (staff == null)
        {
            return NotFound();
        }

        return Ok(staff);
    }

    [HttpPost]
    public async Task<ActionResult> CreateStaffAsync([FromBody] StaffDTO staffDTO)
    {
        try
        {
            await staffRepository.AddStaffDTOAsync(staffDTO);

            return Ok("Staff created successfully.");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"An error occurred while creating the staff. StaffId: {staffDTO.StaffId}");
        }
    }

    // PUT api/<StaffsController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateStaffAsync(string id, [FromBody] StaffDTO staffDTO)
    {
        try
        {
            await staffRepository.UpdateStaffAsync(staffDTO);

            return Ok("Staff updated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"An error occurred while creating the staff. StaffId: {staffDTO.StaffId}");
        }
    }

    // DELETE api/<StaffsController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStaffAsync(string id)
    {
        try
        {
            await staffRepository.DeleteStaffAsync(id);

            return Ok("Staff updated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"Error message: {ex.Message}");
            return StatusCode(500, $"An error occurred while creating the staff. StaffId: {id}");
        }
    }
}
