namespace Application.Common.Users
{
    public sealed record GetUserResponseDTO(Guid Id, string Username, string PictureUrl);
}