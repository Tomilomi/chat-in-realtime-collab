using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .ToListAsync();
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetRecentMessagesAsync(int count)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .OrderByDescending(m => m.Timestamp)
                .Take(count)
                .ToListAsync();
            messages.Reverse(); // revertir el orden de los mensajes para mostrar el mas antiguo primero
            return messages;
        }
    }
}