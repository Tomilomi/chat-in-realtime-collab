using Domain.Errors;
using ErrorOr;
using System.Runtime.CompilerServices;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Represents an application user, including identification, authentication credentials, and profile picture
    /// information.
    /// </summary>
    /// <remarks>The User class is typically created and managed through the static Create method, which
    /// performs necessary validation and encapsulates construction logic. Properties are read-only and can only be set
    /// during object creation, ensuring the immutability of user data after instantiation.</remarks>
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        
        public UserRole Role { get; private set; }
        
        public bool IsBanned { get; private set; }

        //propiades de navegación para el ORM
        public Guid? PictureId { get; private set; }

        public Picture? Picture { get; private set; }

        // public ICollection<Message> SentMessages { get; private set; } = [];
        // public ICollection<Message> ReceivedMessages { get; private set; } = [];
        // Una sola sala

        private User()
        {
            Username = null!;
            Password = null!;
            Picture = null!;
        }

        private User(string username, string password, Picture picture)
        {
            Id = Guid.NewGuid();
            Username = username;
            Password = password;
            Picture = picture;
            Role = UserRole.User;
        }
        
        public void ChangeRole(UserRole role) => Role = role;
        
        public void Ban() => IsBanned = true;
        
        public void Unban() => IsBanned = false;

        /// <summary>
        /// Creates a new User instance with the specified identifier, username, password, and profile picture.
        /// </summary>
        /// <param name="id">The unique identifier to assign to the user.</param>
        /// <param name="username">The username for the user. Cannot be null or empty.</param>
        /// <param name="password">The password for the user. Cannot be null or empty.</param>
        /// <param name="picture">The profile picture to associate with the user. Cannot be null.</param>
        /// <returns>An ErrorOr<User> containing the created User if successful; otherwise, an error describing why the user
        /// could not be created.</returns>
        public static ErrorOr<User> Create(string username, string password, Picture picture) //luego cambiar por pictureId es mas rapido usar la FK
        {
            //validaciones
            List<Error> errors = [];
            errors.AddRange(ValidateUsername(username));
            errors.AddRange(ValidatePassword(password));

            //devolver si hay errores
            if (errors is { Count: > 0 }) { return errors; }

            //creamos la clase
            var result = new User(username, password, picture);
            //retorno
            return result;
        }

        public ErrorOr<Updated> Update(string? newUsername = null,
            string? newPassword = null, Guid? newPictureId = null)
        {
            //TODO cambiar el temita de la picture y hashear la pw
            List<Error> errors = [];

            if (newUsername is not null) errors.AddRange(ValidateUsername(newUsername));
            if (newPassword is not null) errors.AddRange(ValidatePassword(newPassword));

            if (errors is { Count: > 0 }) { return errors; }

            Username = newUsername ?? Username;
            Password = newPassword ?? Password; //todo hashear
            PictureId = newPictureId ?? PictureId;

            return Result.Updated;
        }

        private static List<Error> ValidateUsername(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return [DomainErrors.User.Validation.UsernameEmpty];
            }
            if (username.Length < 3)
            {
                return [DomainErrors.User.Validation.UsernameTooShort];
            }
            return [];
        }

        private static List<Error> ValidatePassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return [DomainErrors.User.Validation.PasswordEmpty];
            }
            if (password.Length < 5)
            {
                return [DomainErrors.User.Validation.PasswordTooShort];
            }
            return [];
        }
        

    }
    
    
}