namespace Application.Common.Users
{
    public sealed record class UserSenderDTO(Guid Id, string Username, string? PictureUrl);
}