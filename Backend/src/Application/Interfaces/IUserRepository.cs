using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task<User?> GetByUserNameAsync(string username);

        Task AddAsync(User user);
    }
}