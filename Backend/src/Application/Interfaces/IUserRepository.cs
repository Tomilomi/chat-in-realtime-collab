using Domain.Entities;
using ErrorOr;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task UpdateAsync(User user);

        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task<User?> GetByUserNameAsync(string username);

        Task AddAsync(User user);
        
        Task<bool> BanAsync(Guid id);
        Task<bool> UnbanAsync(Guid id);
    }
}