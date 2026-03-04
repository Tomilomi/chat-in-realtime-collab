using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            return await _messageRepository.GetAllAsync();
        }

        public async Task SaveMessageAsync(Message message)
        {
            await _messageRepository.AddAsync(message);
        }
        
        public async Task<IEnumerable<Message>> GetMessagesPagedAsync(int page, int pageSize)
        {
            return await _messageRepository.GetMessagesPagedAsync(page, pageSize);
        }
    }
}