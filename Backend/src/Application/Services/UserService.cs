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
        

        public async Task RegisterAsync(string userName, string password)
        {
            var picture = await _pictureRepository.GetDefaultAsync();
            Console.WriteLine($"Picture encontrada: {picture?.Id}");
    
            var user = User.Create(userName, password, picture!);
            Console.WriteLine($"User.IsError: {user.IsError}");
            if (user.IsError)
            {
                Console.WriteLine($"Errores: {string.Join(", ", user.Errors)}");
                return;
            }
            await _userRepository.AddAsync(user.Value);
            Console.WriteLine($"Usuario guardado: {user.Value.Username}");
        }

        public async Task<User?> LoginAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user is null) return null;
            return user.Password != password ? null : user;
        }
        
        
        
    }
}