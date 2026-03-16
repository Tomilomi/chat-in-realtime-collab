namespace chat_in_realtime.Notifications;

using Application.Interfaces;
using chat_in_realtime.Hubs;
using Microsoft.AspNetCore.SignalR;

public class ChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatNotificationService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task KickUserAsync(string userId, string reason)
    {
        Console.WriteLine($"Intentando kickear userId: {userId}");
        Console.WriteLine($"Conexiones actuales: {string.Join(", ", ChatHub.UserConnections.Keys)}");
    
        if (ChatHub.UserConnections.TryGetValue(userId, out var connectionId))
        {
            Console.WriteLine($"ConnectionId encontrado: {connectionId}");
            await _hubContext.Clients.Client(connectionId).SendAsync("Kicked", reason);
        }
        else
        {
            Console.WriteLine("No se encontró la conexión");
        }
    }
    
    public async Task NotifyMessageDeletedAsync(Guid messageId)
    {
        await _hubContext.Clients.All.SendAsync("MessageDeleted", messageId);
    }
}