using FurnitureStore.Server.Models.Documents;
using FurnitureStore.Shared;

namespace FurnitureStore.Server.Interfaces
{
    public interface IStaffRepository
    {
        Task AddStaffDocumentAsync(StaffDocument item);
        Task AddStaffDTOAsync(StaffDTO staffDTO);
        Task DeleteStaffAsync(string id);
        Task<string> GetNewStaffIdAsync();
        Task<StaffDTO> GetStaffDTOByIdAsync(string id);
        Task<IEnumerable<StaffDTO>> GetStaffDTOsAsync();
        Task UpdateStaffAsync(StaffDTO staffDTO);
    }
}
