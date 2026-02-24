using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace chat_in_realtime.Hubs;

public class ChatHub : Hub
{
    private static readonly List<object> _messages = [];
    private readonly IMessageService _messageService;

    public ChatHub(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task SendMessage(string username, string content)
    {
        _messages.Add(new { username, content });
        await Clients.All.SendAsync("ReceiveMessage", username, content);
    }

    public async Task LoadMessages()
    {
        await Clients.Caller.SendAsync("LoadMessages", _messages);
    }
}