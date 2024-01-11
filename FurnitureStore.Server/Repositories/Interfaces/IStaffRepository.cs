using FurnitureStore.Server.Models.BindingModels.PasswordModels;
using FurnitureStore.Server.Models.Documents;

namespace FurnitureStore.Server.Repositories.Interfaces;

public interface IStaffRepository
{
    Task AddStaffDocumentAsync(StaffDocument item);
    Task AddStaffDTOAsync(StaffDTO staffDTO);
    Task DeleteStaffAsync(string id);
    Task<string> GetNewStaffIdAsync();
    Task<StaffDTO> GetStaffDTOByIdAsync(string id);
    Task<StaffDTO> GetStaffDTOByCredentials(LoginModel data);
    Task UpdatePasswordAsync(UpdatePasswordModel data);
    Task<IEnumerable<StaffDTO>> GetStaffDTOsAsync();
    Task UpdateStaffAsync(StaffDTO staffDTO);
}
