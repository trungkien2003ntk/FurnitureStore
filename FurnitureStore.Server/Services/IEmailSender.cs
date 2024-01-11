using FurnitureStore.Server.Models.Emails;

namespace FurnitureStore.Server.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}
