using Application.Common.Users;

namespace Application.Common
{
    public sealed record GetAllUsersResponseDTO(IEnumerable<GetUserResponseDTO> Users);
}