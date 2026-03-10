using Application.Interfaces.User;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Picture)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userName);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Picture)
                .AsNoTracking() //evita el seguimiento de las entidades para mejorar el rendimiento
                .ToListAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> BanAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false;
            user.Ban();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnbanAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false;
            user.Unban();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}