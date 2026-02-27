using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid id);
    }
}