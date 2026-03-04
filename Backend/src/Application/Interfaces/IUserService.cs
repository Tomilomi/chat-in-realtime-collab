using Application.Common;
using Application.Common.Users;
using Domain.Entities;
using ErrorOr;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<ErrorOr<Updated>> UpdateAsync(Guid id, UserUpdateRequestDTO request);

        Task<ErrorOr<GetAllUsersResponseDTO>> GetAllAsync();

        Task<ErrorOr<User>> GetByIdAsync(Guid id);

        Task RegisterAsync(string username, string password);

        Task<ErrorOr<User>> LoginAsync(string username, string password);
    }
}