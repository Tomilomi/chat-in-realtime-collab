using Application.Common;
using Domain.Entities;
using ErrorOr;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<ErrorOr<GetAllUsersResponseDTO>> GetAllUsersAsync();

        Task<ErrorOr<User>> GetUserByIdAsync(Guid id);

        Task RegisterAsync(string username, string password);

        Task<ErrorOr<User>> LoginAsync(string username, string password);
    }
}