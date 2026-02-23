using Domain.Entity;

namespace Application.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task SendMessageAsync(Message message);
    }
}