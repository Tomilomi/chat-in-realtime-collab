namespace Application.Common
{
    /// <summary>
    /// Mensaje de entrada desde el cliente al servidor.
    /// </summary>
    public sealed record class SendMessageDTO(string Content);
}