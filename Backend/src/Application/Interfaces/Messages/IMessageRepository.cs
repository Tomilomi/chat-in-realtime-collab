using Domain.Entities;

namespace Application.Interfaces.Messages
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllAsync();

        Task AddAsync(Message message);
        
        Task<IEnumerable<Message>> GetMessagesPagedAsync(int page, int pageSize);
        
        Task<bool> DeleteAsync(Guid id);
    }
}