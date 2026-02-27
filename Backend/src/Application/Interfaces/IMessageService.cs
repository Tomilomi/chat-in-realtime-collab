using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();

        Task SaveMessageAsync(Message message);

        Task<IEnumerable<Message>> GetRecentMessagesAsync(int count);
    }
}