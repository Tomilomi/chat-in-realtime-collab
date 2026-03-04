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
        
        
        public async Task<IEnumerable<Message>> GetMessagesPagedAsync(int page, int pageSize)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .OrderByDescending(m => m.Timestamp)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
            messages.Reverse();
            return messages;
        }
    }
}