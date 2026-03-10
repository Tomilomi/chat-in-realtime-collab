using Application.Common;
using Application.Common.Users;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces.Users;
using Application.Interfaces.Messages;

namespace chat_in_realtime.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly IChatNotificationService _chatNotificationService;
    private static readonly HashSet<String> _connectedUsers = [];
    private static readonly Dictionary<string, DateTime> _lastMessageTime = new();
    private const int PageSize = 20;

    public ChatHub(IMessageService messageService, IUserService userService, IChatNotificationService chatNotificationService)
    {
        _messageService = messageService;
        _userService = userService;
        _chatNotificationService = chatNotificationService;
    }

    public async Task SendMessage(SendMessageDTO messageIn)
    {
        if (string.IsNullOrWhiteSpace(messageIn.Content))
            throw new HubException("El mensaje no puede estar vacío");

        //identificar el usuario
        // NECESITA JWT
        string userIdString = Context.UserIdentifier ?? throw new HubException("No autorizado");
        Guid userId = Guid.Parse(userIdString);

        if (_lastMessageTime.TryGetValue(userIdString, out var lastTime))
        {
            if ((DateTime.UtcNow - lastTime).TotalSeconds < 1)
                throw new HubException("Estás enviando mensajes muy rápido");
        }
        _lastMessageTime[userIdString] = DateTime.UtcNow;

        //buscarlo en la bd
        //se puede mejorar el manejo de erorres
        var result = await _userService.GetByIdAsync(userId);
        if (result.IsError)
        {
            throw new HubException("Usuario no encontrado.");
        }

        var user = result.Value;
        //entidad para la bd
        var newMessage = new Message(
            sender: user,
            content: messageIn.Content
            );

        //guardar en bd
        await _messageService.SaveMessageAsync(newMessage);

        //dto para el hub
        var messageToBroadcast = new MessageReceivedDTO(
            Id: newMessage.Id,
            Content: newMessage.Content,
            Timestamp: newMessage.Timestamp,
            Sender: new UserSenderDTO
            (
                Id: user.Id,
                Username: user.Username,
                PictureUrl: user.Picture?.Url
            )

        );
        //mensaje enviado al hub
        await Clients.All.SendAsync("ReceiveMessage", messageToBroadcast);
    }

    public static readonly Dictionary<string, string> UserConnections = new();

    public async Task LoadMessages(int page = 0)
    {
        var pastMessages = await _messageService.GetMessagesPagedAsync(page, PageSize);

        var messagesToLoad = pastMessages.Select(m => new MessageReceivedDTO(
            Id: m.Id,
            Content: m.Content,
            Timestamp: m.Timestamp,
            Sender: new UserSenderDTO(
                Id: m.Sender.Id,
                Username: m.Sender.Username,
                PictureUrl: m.Sender.Picture?.Url
            )
        )).ToList();

        await Clients.Caller.SendAsync("LoadMessages", messagesToLoad, page);
    }

    // Users conected log

    public override async Task OnConnectedAsync()
    {
        string userIdString = Context.UserIdentifier ?? throw new HubException("No autorizado");
        var user = await _userService.GetByIdAsync(Guid.Parse(userIdString));
        if (user.IsError) throw new HubException("Usuario no encontrado");

        if (user.Value.IsBanned) throw new HubException("Usuario baneado");

        UserConnections[userIdString] = Context.ConnectionId;
        _connectedUsers.Add(user.Value.Username);
        await Clients.All.SendAsync("UserConnected", user.Value.Username);
        await Clients.All.SendAsync("UpdateConnectedUsers", _connectedUsers);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string userIdString = Context.UserIdentifier ?? throw new HubException("No autorizado");
        var user = await _userService.GetByIdAsync(Guid.Parse(userIdString));
        if (!user.IsError)
        {
            UserConnections.Remove(user.Value.Username);
            _connectedUsers.Remove(user.Value.Username);
            await Clients.All.SendAsync("UserDisconnected", user.Value.Username);
            await Clients.All.SendAsync("UpdateConnectedUsers", _connectedUsers);
        }
        await base.OnDisconnectedAsync(exception);
    }

    // Typing log

    public async Task StartTyping()
    {
        string userIdString = Context.UserIdentifier ?? throw new HubException("No autorizado");
        var user = await _userService.GetByIdAsync(Guid.Parse(userIdString));
        if (user.IsError) return;

        await Clients.Others.SendAsync("UserTyping", user.Value.Username);
    }

    public async Task StopTyping()
    {
        string userIdString = Context.UserIdentifier ?? throw new HubException("No autorizado");
        var user = await _userService.GetByIdAsync(Guid.Parse(userIdString));
        if (user.IsError) return;

        await Clients.Others.SendAsync("UserStoppedTyping", user.Value.Username); // ← esto estaba mal
    }

    // to kick users

    public async Task KickUser(Guid userId)
    {
        string userIdString = Context.UserIdentifier ?? throw new HubException("No autorizado");
        var caller = await _userService.GetByIdAsync(Guid.Parse(userIdString));
        if (caller.IsError) throw new HubException("No autorizado");
        if (caller.Value.Role != UserRole.Admin && caller.Value.Role != UserRole.Moderator)
            throw new HubException("No tenés permisos para kickear usuarios");

        await _chatNotificationService.KickUserAsync(userId.ToString(), "Fuiste kickeado por un moderador");
    }
}