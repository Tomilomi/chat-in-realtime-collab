namespace Application.Common
{
    /// <summary>
    /// Mensaje de salida desde el servidor al cliente, representando un mensaje recibido en el chat.
    /// </summary>

    public sealed record MessageReceivedDTO(Guid Id,
        string Content, DateTime Timestamp, string SenderUsername);
}