using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task SendMessageAsync(Message message);
    }
}