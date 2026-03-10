using Application.Common;
using Application.Common.Users;
using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Pictures;
using Application.Interfaces.Users;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPictureRepository _pictureRepository;
        private readonly IChatNotificationService _chatNotificationService;

        public UserService(IUserRepository userRepository, IPictureRepository pictureRepository, IChatNotificationService chatNotificationService)
        {
            _userRepository = userRepository;
            _pictureRepository = pictureRepository;
            _chatNotificationService = chatNotificationService;
        }

        public async Task<ErrorOr<User>> GetByIdAsync(Guid id)
        {
            var result = await _userRepository.GetByIdAsync(id);
            if (result == null) { return DomainErrors.User.NotFound; }
            return result;
        }

        public async Task RegisterAsync(string userName, string password)
        {
            var existing = await _userRepository.GetByUserNameAsync(userName);
            if (existing is not null) return;

            var picture = await _pictureRepository.GetDefaultAsync();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = User.Create(userName, hashedPassword, picture!);
            if (user.IsError) return;
            await _userRepository.AddAsync(user.Value);
        }

        public async Task<ErrorOr<User>> LoginAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user is null) return DomainErrors.User.NotFound;
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return DomainErrors.User.Bussiness.IncorrectPassword;
            }
            return user;
        }

        public async Task<ErrorOr<GetAllUsersResponseDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = users.Select(user => user.ToDto()).ToList();
            var result = new GetAllUsersResponseDTO(dtos);
            return result;
        }

        public async Task<ErrorOr<Updated>> UpdateAsync(Guid id, UserUpdateRequestDTO request)
        {
            var resultFind = await GetByIdAsync(id);
            if (resultFind.IsError) return resultFind.Errors;

            var user = resultFind.Value;

            var resultUpdate = user.Update(request.Username, request.Password, request.PictureId);
            if (resultUpdate.IsError) return resultUpdate.Errors;

            await _userRepository.UpdateAsync(user);
            return Result.Updated;
        }

        public async Task<bool> BanAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null) return false;
            user.Ban();
            await _userRepository.UpdateAsync(user);
            await _chatNotificationService.KickUserAsync(id.ToString(), "Fuiste baneado por un administrador");
            return true;
        }

        public async Task<bool> UnbanAsync(Guid id)
        {
            return await _userRepository.UnbanAsync(id);
        }

        public async Task<bool> ChangeRoleAsync(Guid userId, UserRole role)
        {
            var result = await _userRepository.GetByIdAsync(userId);
            if (result is null) return false;
            result.ChangeRole(role);
            await _userRepository.UpdateAsync(result);
            return true;
        }

        public async Task<ErrorOr<IEnumerable<UserProfileDTO>>> GetAllProfilesAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserProfileDTO(u.Username, u.Picture?.Url)).ToList();
        }
    }
}