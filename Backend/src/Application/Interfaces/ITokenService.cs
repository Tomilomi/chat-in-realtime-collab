using Application.Common.Auth;
using ErrorOr;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        ErrorOr<string> GenerateToken(GenerateTokenRequestDTO request);
    }
}