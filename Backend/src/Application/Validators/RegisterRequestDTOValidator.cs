using Application.Common.Auth;
using chat_in_realtime.Extensions;
using Domain.Errors;
using FluentValidation;

namespace Application.Validators
{
    public class RegisterRequestDTOValidator : AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestDTOValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithError(DomainErrors.User.Validation.UsernameEmpty)
                .MinimumLength(3).WithError(DomainErrors.User.Validation.UsernameTooShort)
                .MaximumLength(20).WithError(DomainErrors.User.Validation.UsernameTooLong);
        }
    }
}