using Domain.Entity;

namespace Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllAsync();
        Task AddAsync(Message message);
    }
}