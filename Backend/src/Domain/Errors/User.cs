using ErrorOr;

namespace Domain.Errors
{
    public static partial class DomainErrors
    {
        /// <summary>
        /// Provides predefined private validation errors related to user input,
        /// such as invalid username, password, or picture.
        /// </summary>
        /// <remarks>This static class contains reusable error instances that can be returned when
        /// user-related validation fails. These errors are intended to standardize error handling for user input
        /// validation across the application.</remarks>
        public static class User
        {
            public static Error InvalidUsername => Error.Validation(
                code: "User.InvalidUsername",
                description: "The provided username is invalid."
            );

            public static Error UsernameTooShort => Error.Validation(
                code: "User.UsernameTooShort",
                description: "The username is too short."
            );

            public static Error UsernameEmpty => Error.Validation(
                code: "User.UsernameEmpty",
                description: "The username cannot be null or whitespace."
            );

            public static Error InvalidPassword => Error.Validation(
                code: "User.InvalidPassword",
                description: "The provided password is invalid."
            );

            public static Error PasswordEmpty => Error.Validation(
                code: "User.PasswordEmpty",
                description: "The password cannot be null or whitespace."
            );

            public static Error InvalidPicture => Error.Validation(
                code: "User.InvalidPicture",
                description: "The provided picture is invalid."
            );
        }
    }
}