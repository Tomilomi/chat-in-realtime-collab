using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(Guid id);
        Task RegisterAsync(string username, string password);
        Task<User?> LoginAsync(string username, string password);
    }
}