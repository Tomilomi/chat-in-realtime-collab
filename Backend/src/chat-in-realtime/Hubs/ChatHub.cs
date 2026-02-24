
using Microsoft.AspNetCore.SignalR;

namespace chat_in_realtime.Hubs;

using Application.Interfaces;


public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    public ChatHub(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task SendMessage(string username,string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", username, message);
    }

    public async Task LoadMessages()
    {
        var messages = await _messageService.GetAllMessagesAsync();
        await Clients.Caller.SendAsync("LoadMessages", messages);
    }
}