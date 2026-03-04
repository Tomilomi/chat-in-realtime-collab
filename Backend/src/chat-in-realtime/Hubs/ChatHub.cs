using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace chat_in_realtime.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;

    public ChatHub(IMessageService messageService, IUserService userService)
    {
        _messageService = messageService;
        _userService = userService;
    }

    public async Task SendMessage(SendMessageDTO messageIn)
    {
        //identificar el usuario
        // NECESITA JWT
        string userIdString = Context.UserIdentifier ?? throw new HubException("No autorizado");
        Guid userId = Guid.Parse(userIdString);

        //buscarlo en la bd
        //se puede mejorar el manejo de erorres
        var result = await _userService.GetUserByIdAsync(userId);
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
                Username: user.Username
            )

        );
        //mensaje enviado al hub
        await Clients.All.SendAsync("ReceiveMessage", messageToBroadcast);
    }

    public async Task LoadMessages()
    {
        //traerlos de la bd
        var pastMessages = await _messageService.GetRecentMessagesAsync(20);

        //mapearlos a dto
        var messagesToLoad = pastMessages.Select(m => new MessageReceivedDTO(
            Id: m.Id,
            Content: m.Content,
            Timestamp: m.Timestamp,
            Sender: new UserSenderDTO(
                    Id: m.Sender.Id,
                    Username: m.Sender.Username
                    )
                )
            ).ToList();

        await Clients.Caller.SendAsync("LoadMessages", messagesToLoad);
    }
}