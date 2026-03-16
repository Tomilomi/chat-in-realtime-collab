using Application.Common;
using Application.Common.Users;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces.Users;
using Application.Interfaces.Messages;
using FluentValidation;
using ErrorOr;

namespace chat_in_realtime.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly IChatNotificationService _chatNotificationService;
    private readonly IValidator<SendMessageDTO> _messageValidator;
    private static readonly HashSet<String> _connectedUsers = [];
    private static readonly Dictionary<string, DateTime> _lastMessageTime = new();
    private const int PageSize = 20;

    public ChatHub(IMessageService messageService,
        IUserService userService, IChatNotificationService chatNotificationService,
        IValidator<SendMessageDTO> messageValidator)
    {
        _messageService = messageService;
        _userService = userService;
        _chatNotificationService = chatNotificationService;
        _messageValidator = messageValidator;
    }

    public async Task<ErrorOr<Success>> SendMessage(SendMessageDTO messageIn)
    {
        //identificar el usuario
        // NECESITA JWT
        string userIdString = Context.UserIdentifier!;
        Guid userId = Guid.Parse(userIdString);

        //obtener usuario
        var userResult = await _userService.GetByIdAsync(userId);
        if (userResult.IsError) return userResult.Errors;

        var user = userResult.Value;

        //crear y guardar mensaje
        var newMessage = new Message(sender: user, content: messageIn.Content);
        await _messageService.SaveMessageAsync(newMessage);

        //mapear y transmitir
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
        
        
        await Clients.Others.SendAsync("UserStoppedTyping", user.Username);

        return Result.Success;
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
        var userResult = await _userService.GetByIdAsync(Guid.Parse(userIdString));
        if (userResult.IsError) return;

        var user = userResult.Value;
        await Clients.Others.SendAsync("UserStoppedTyping", user.Username); // ← esto estaba mal
    }

    // to kick users

    public async Task<ErrorOr<Success>> KickUser(Guid userId)
    {
        string userIdString = Context.UserIdentifier ?? throw new HubException("No autorizado");
        var callerResult = await _userService.GetByIdAsync(Guid.Parse(userIdString));
        if (callerResult.IsError) return callerResult.Errors;

        var caller = callerResult.Value;

        if (caller.Role != UserRole.Admin && caller.Role != UserRole.Moderator)
            return Error.Unauthorized("Chat.Permission", "No tenes permisos para kickear usuarios");

        await _chatNotificationService.KickUserAsync(userId.ToString(), "Fuiste kickeado por un moderador");

        return Result.Success;
    }
}