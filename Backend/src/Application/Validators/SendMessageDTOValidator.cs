using Application.Common;
using chat_in_realtime.Extensions;
using Domain.Errors;
using FluentValidation;

namespace Application.Validators
{
    public class SendMessageDTOValidator : AbstractValidator<SendMessageDTO>
    {
        public SendMessageDTOValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithError(DomainErrors.Message.Validation.Blank)
                .MaximumLength(500).WithError(DomainErrors.Message.Validation.TooLong);
        }
    }
}