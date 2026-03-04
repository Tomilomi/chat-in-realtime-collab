using Application.Common.Users;
using Domain.Entities;

namespace Application.Extensions
{
    /// <summary>
    /// Clase de extension para añadir métodos de mapeo
    /// </summary>
    public static class UserMappingExtensions
    {
        public static GetUserResponseDTO ToDto(this User user)
        {
            return new GetUserResponseDTO(
                Id: user.Id,
                Username: user.Username,
                PictureId: user.PictureId
            );
        }
    }
}