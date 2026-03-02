using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        
        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
        

        public async Task RegisterAsync(string userName, string password, Guid userId)
        {
            // Implementar en el picture repository
            
            var picture = await _pictureRepository.GetByIdAsync(pictureId)
                          ?? await _pictureRepository.GetDefaultAsync();
            
            
            if (picture is null) return;
            var user = User.Create(userName, password, picture);
            if (user.IsError) return;
            await _userRepository.AddAsync(user.Value);
        }

        public async Task<User?> LoginAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user is null) return null;
            if (user.Password != password) return null;
            return user;
        }
        
        
        
    }
}