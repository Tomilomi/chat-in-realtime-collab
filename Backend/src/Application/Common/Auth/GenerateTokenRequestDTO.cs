using Domain.Enums;

namespace Application.Common.Auth
{
    public sealed record GenerateTokenRequestDTO(Guid UserId, string Username, UserRole Role);
}