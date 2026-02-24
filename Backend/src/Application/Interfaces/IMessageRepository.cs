using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllAsync();

        Task AddAsync(Message message);
    }
}