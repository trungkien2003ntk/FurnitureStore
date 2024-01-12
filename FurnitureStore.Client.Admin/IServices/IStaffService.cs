using FurnitureStore.Shared.DTOs;

namespace FurnitureStore.Client.Admin.IServices
{
    public interface IStaffService
    {
        Task<StaffDTO> LoginStaffAsync(string email, string password);
    }
}
