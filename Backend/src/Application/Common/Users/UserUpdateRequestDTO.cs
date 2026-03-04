namespace Application.Common.Users
{
    public sealed record UserUpdateRequestDTO(string Username, string Password, Guid PictureId);
}