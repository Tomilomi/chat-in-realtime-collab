using Application.Interfaces;
using Domain.Entities;

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
        
        
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
        

        public async Task RegisterAsync(string userName, string password, Guid pictureId)
        {
            
            var picture = await _pictureRepository.GetByIdAsync(pictureId)
                          ?? await _pictureRepository.GetDefaultAsync();
            
            var user = User.Create(userName, password, picture!);
            if (user.IsError) return;
            await _userRepository.AddAsync(user.Value);
        }

        public async Task<User?> LoginAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user is null) return null;
            return user.Password != password ? null : user;
        }
        
        
        
    }
}