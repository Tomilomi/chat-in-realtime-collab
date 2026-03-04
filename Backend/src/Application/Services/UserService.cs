using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Domain.Errors;
using ErrorOr;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPictureRepository _pictureRepository;

        public UserService(IUserRepository userRepository, IPictureRepository pictureRepository)
        {
            _userRepository = userRepository;
            _pictureRepository = pictureRepository;
        }

        public async Task<ErrorOr<User>> GetUserByIdAsync(Guid id)
        {
            var result = await _userRepository.GetByIdAsync(id);
            if (result == null) { return DomainErrors.User.NotFound; }
            return result;
        }

        public async Task RegisterAsync(string userName, string password)
        {
            var picture = await _pictureRepository.GetDefaultAsync();

            var user = User.Create(userName, password, picture!);
            if (user.IsError) return;
            await _userRepository.AddAsync(user.Value);
        }

        public async Task<ErrorOr<User>> LoginAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user is null) return DomainErrors.User.NotFound;
            if (user.Password != password) { return DomainErrors.User.Bussiness.IncorrectPassword; }
            return user;
        }

        public async Task<ErrorOr<GetAllUsersResponseDTO>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }
    }
}