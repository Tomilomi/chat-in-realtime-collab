using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class UserService : IUserService
    {
        public Task<User> GetUserByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}