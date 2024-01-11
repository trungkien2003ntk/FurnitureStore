using MimeKit;

namespace FurnitureStore.Server.Models.Emails
{
    public class Message(IEnumerable<string> to, string subject, string content)
    {
        public List<MailboxAddress> To { get; set; } = [.. to.Select(x => new MailboxAddress(string.Empty, x))];
        public string Subject { get; set; } = subject;
        public string Content { get; set; } = content;
    }
}
