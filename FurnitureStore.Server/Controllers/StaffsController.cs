using Microsoft.AspNetCore.Mvc;
using FurnitureStore.Shared;
using FurnitureStore.Server.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FurnitureStore.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly ILogger<StaffsController> _logger;
        private readonly IStaffRepository _staffRepository;

        public StaffsController(ILogger<StaffsController> logger, IStaffRepository staffRepository)
        {
            this._logger = logger;
            this._staffRepository = staffRepository;
        }

        // GET: api/<StaffsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffDTO>>> GetStaffDTOsAsync()
        {
            var staffs = await _staffRepository.GetStaffDTOsAsync();

            if (staffs == null || !staffs.Any())
            {
                return NotFound();
            }

            return Ok(staffs);
        }

        [HttpGet("newId")]
        public async Task<ActionResult<string>> GetNewStaffIdAsync()
        {
            string newId = await _staffRepository.GetNewStaffIdAsync();

            return Ok(newId);
        }

        // GET api/<StaffsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDTO>> GetStaffDTOByIdAsync(string id)
        {
            var staff = await _staffRepository.GetStaffDTOByIdAsync(id);

            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        // POST api/<StaffsController>
        [HttpPost]
        public async Task<ActionResult> CreateStaffAsync([FromBody] StaffDTO staffDTO)
        {
            try
            {
                await _staffRepository.AddStaffDTOAsync(staffDTO);

                return Ok("Staff created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error message: {ex.Message}");
                return StatusCode(500, $"An error occurred while creating the staff. StaffId: {staffDTO.StaffId}");
            }
        }

        // PUT api/<StaffsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStaffAsync(string id, [FromBody] StaffDTO staffDTO)
        {
            try
            {
                await _staffRepository.UpdateStaffAsync(staffDTO);

                return Ok("Staff updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error message: {ex.Message}");
                return StatusCode(500, $"An error occurred while creating the staff. StaffId: {staffDTO.StaffId}");
            }
        }

        // DELETE api/<StaffsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStaffAsync(string id)
        {
            try
            {
                await _staffRepository.DeleteStaffAsync(id);

                return Ok("Staff updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error message: {ex.Message}");
                return StatusCode(500, $"An error occurred while creating the staff. StaffId: {id}");
            }
        }
    }
}
