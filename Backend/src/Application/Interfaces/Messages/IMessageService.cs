using Domain.Entities;

namespace Application.Interfaces.Messages
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();

        Task SaveMessageAsync(Message message);

        Task<bool> DeleteMessageAsync(Guid id);

        Task<IEnumerable<Message>> GetMessagesPagedAsync(int page, int pageSize);
    }
}