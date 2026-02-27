using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllAsync();

        Task<IEnumerable<Message>> GetRecentMessagesAsync(int count);

        Task AddAsync(Message message);
    }
}